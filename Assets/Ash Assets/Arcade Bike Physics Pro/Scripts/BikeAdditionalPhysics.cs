using System;
using UnityEngine;
using UnityEngine.Events;
using ArcadeBP_Pro;
using System.Threading;
using UnityEditor.Experimental.GraphView;

public class BikeAdditionalPhysics : MonoBehaviour
{
    [Tooltip("Reference to the ArcadeBikeControllerPro script.")]
    public ArcadeBikeControllerPro bikeController;

    [Tooltip("Reference to the CameraController script.")]
    public CameraController cameraController;

    [Tooltip("Prefab for the Dummy bike.")]
    public GameObject dummyBikePrefab;

    [Tooltip("Prefab for the character ragdoll.")]
    public GameObject characterRagdollPrefab;

    [Tooltip("Animator component of the animated character.")]
    public Animator characterAnimator;

    public int impactCeiling;
    [Tooltip("Threshold of impact force to activate ragdoll.")]
    public float impactThresholdToFall = 10f;
    
    float crashImpact;
    public float verticalImpactThreshold = 10;

    [Tooltip("Ignore collisions with the bottom part of the bike collider.")]
    public bool ignoreBottomCollision = true;

    public event Action onDislodged=delegate { };

    private Rigidbody bikeRigidbody;
    private bool isRagdollActivated = false;
    private GameObject bikeRagdollInstance;
    float damage;
    public Transform GetRagdollBikeTransform() { return bikeRagdollInstance.transform; }
    public GameObject characterRagdollInstance { get; private set; }
    private Collider bikeCollider;

    private void Awake()
    {
        bikeRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        bikeCollider = bikeController.bikeReferences.collider;
        onDislodged+= SetCameraTargetToRagdoll;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isRagdollActivated) 
        {
            crashImpact = 0;
            return; 
        }

        crashImpact = collision.impulse.magnitude / bikeRigidbody.mass;
        var wheelieTransform = bikeController.bikeReferences.wheelieTransform;
        //Debug.Log("Impact " + crashImpact);
        Debug.Break();
        if (!bikeController.isDoingWheelie &&
            !bikeController.isDoingStoppie)
        {
            Debug.Log("Não deveria... ");
            float clampedImpact = Mathf.Clamp(crashImpact, 0, impactCeiling);
            damage = Mathf.Lerp(0, 100, clampedImpact / impactCeiling);
            

            var crashOutputDegree = Mathf.Lerp(0, 35, (crashImpact - 2) / (impactThresholdToFall - 2));

            wheelieTransform.localRotation = Quaternion.Euler(crashOutputDegree, 0, 0);
        }
        else if (bikeController.isDoingWheelie)
        {
            float clampedImpact = Mathf.Clamp(crashImpact, 0, impactCeiling);
            damage = Mathf.Lerp(0, 40, clampedImpact / (impactCeiling/2));

            Debug.Log("Wheelie angle "+ bikeController.wheelieAngle);
            float crashOutputDegree = 0;

            if (bikeController.wheelieAngle <= 15) 
                crashOutputDegree = Mathf.Lerp (0, -25, crashImpact / bikeController.wheelieAngle);
            else
                crashOutputDegree = Mathf.Lerp(0, 25, crashImpact / bikeController.wheelieAngle);

            Debug.Log("Degree on wheelie after crash " + crashOutputDegree);
            wheelieTransform.localRotation = Quaternion.Euler(crashOutputDegree, 0, 0);

            
        }
        bikeController.DecreaseCondition((int)damage);
        if (crashImpact > impactThresholdToFall)
            DislodgePlayer();
        return;

        Vector3 localContactPoint = bikeController.bikeReferences.
            leanTransform.InverseTransformPoint(collision.contacts[0].point);
        Vector3 bikeCenter = bikeController.bikeReferences.
            leanTransform.InverseTransformPoint(bikeCollider.bounds.center);
        //Debug.Log("Local contact point " + localContactPoint);
        //Debug.Log("Bike center " + bikeCenter);
        //Debug.Log("Collision contacts size " + collision.contacts.Length);
        if (ignoreBottomCollision)
        {
            if (localContactPoint.y > bikeCenter.y)
            {
                //Debug.Log("Vertical collision impact " + collision.impulse.magnitude / bikeRigidbody.mass);
                //Debug.Log("Vertical impact threshold " + verticalImpactThreshold);
                if (collision.impulse.magnitude / bikeRigidbody.mass > verticalImpactThreshold)
                {
                    DislodgePlayer();
                }
            }
        }
        else
        {
            Debug.Log("Collision impact " + collision.impulse.magnitude / bikeRigidbody.mass);
            Debug.Log("Impact threshold " + impactThresholdToFall);
            if (collision.impulse.magnitude / bikeRigidbody.mass > impactThresholdToFall)
                DislodgePlayer();
        }
    }

    public void DislodgePlayer(bool isDoingWheelie = false, bool isStoppie = false)
    {
        isRagdollActivated = true;

        Transform bikeTransform = bikeController.bikeReferences.leanTransform;

        // Instantiate the ragdolls
        bikeRagdollInstance = Instantiate(dummyBikePrefab, bikeTransform.position, bikeTransform.rotation);
        characterRagdollInstance = Instantiate(characterRagdollPrefab, bikeTransform.position, bikeTransform.rotation);

        // Match the ragdoll bones' rotations to the character's bones
        /*Animator ragdollAnimator = characterRagdollInstance.GetComponent<Animator>();
        foreach (HumanBodyBones bone in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
        {
            if (bone == HumanBodyBones.LastBone) continue;

            Transform characterBoneTransform = characterAnimator.GetBoneTransform(bone);
            Transform ragdollBoneTransform = ragdollAnimator.GetBoneTransform(bone);

            if (characterBoneTransform != null && ragdollBoneTransform != null)
            {
                ragdollBoneTransform.rotation = characterBoneTransform.rotation;
            }
        }*/

        // Match velocities and forces
        Rigidbody[] bikeRagdollRigidbodies = bikeRagdollInstance.GetComponentsInChildren<Rigidbody>();
        Rigidbody[] characterRagdollRigidbodies = characterRagdollInstance.GetComponentsInChildren<Rigidbody>();

        Vector3 bikeVelocity = bikeRigidbody.velocity;
        Vector3 bikeAngularVelocity = bikeRigidbody.angularVelocity;

        foreach (Rigidbody rb in bikeRagdollRigidbodies)
        {
            if (isDoingWheelie)
                rb.velocity = bikeVelocity*2;
            else
                rb.velocity = bikeVelocity;
            rb.angularVelocity = bikeAngularVelocity;
        }

        foreach (Rigidbody rb in characterRagdollRigidbodies)
        {
            if (!isDoingWheelie)
            {
                rb.velocity = bikeVelocity;
                rb.angularVelocity = bikeAngularVelocity;
            }
            else
            {
                rb.velocity = -bikeVelocity;
                Debug.Log(bikeVelocity);
                rb.angularVelocity = -bikeAngularVelocity;
            }
        }

        if (isDoingWheelie)
        {
            var wheelieMaxSpeed = bikeController.bikeSettings.wheelieMaxSpeed;
            float clampedSpeed = Mathf.Clamp(bikeController.speedKmph,
                0, (wheelieMaxSpeed * 3.6f));
            damage = Mathf.Lerp(0, 100, clampedSpeed / (wheelieMaxSpeed * 3.6f));
        }
        else
        {
            damage = Mathf.Lerp(0, 100, bikeController.speedKmph/
                bikeController.stoppieAngle);
        }


        bikeController.DecreaseCondition((int)damage);
        gameObject.SetActive(false);
        onDislodged();
    }

    public void CleanRagdolls()
    {
        if (!isRagdollActivated)
            return;

        // Destroy the ragdoll instances
        Destroy(bikeRagdollInstance);
        Destroy(characterRagdollInstance);

        // Re-enable the original bike
        /*gameObject.SetActive(true);
        bikeController.canAccelerate = true;
        bikeController.bikeAudio.engineSound.pitch = bikeController.bikeAudio.minPitch;
        bikeController.bikeReferences.LeanTransform.localRotation = Quaternion.identity;
        bikeRigidbody.velocity = Vector3.zero;
        bikeRigidbody.angularVelocity = Vector3.zero;*/

        isRagdollActivated = false;

    }

    public void SetCameraTargetToRagdoll()
    {
        var targetTransform = transform;
        if (cameraController.currentCameraIndex != 2)
        {
            targetTransform = characterRagdollInstance.GetComponent<Animator>().
                GetBoneTransform(HumanBodyBones.Hips);
        }
        else
        {
            targetTransform = characterRagdollInstance.GetComponent<Animator>().
                GetBoneTransform(HumanBodyBones.Head);
        }
        cameraController.SetCameratarget(targetTransform, targetTransform);
    }


    public void ForceActivateRagdoll()
    {
        if (!isRagdollActivated)
        {
            DislodgePlayer();
        }
    }

}
