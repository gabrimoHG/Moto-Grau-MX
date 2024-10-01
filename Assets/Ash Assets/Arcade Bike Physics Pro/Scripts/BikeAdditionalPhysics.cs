using System;
using UnityEngine;
using UnityEngine.Events;
using ArcadeBP_Pro;
using System.Threading;

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

    [Tooltip("Threshold of impact force to activate ragdoll.")]
    public float impactThreshold = 10f;

    [Tooltip("Ignore collisions with the bottom part of the bike collider.")]
    public bool IgnoreBottomCollision = true;

    public event Action onDislodged=delegate { };

    private Rigidbody bikeRigidbody;
    private bool isRagdollActivated = false;
    private GameObject bikeRagdollInstance;
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
        if (isRagdollActivated) return;

        Vector3 localContactPoint = bikeController.bikeReferences.leanTransform.InverseTransformPoint(collision.contacts[0].point);
        Vector3 bikeCenter = bikeController.bikeReferences.leanTransform.InverseTransformPoint(bikeCollider.bounds.center);

        if (IgnoreBottomCollision)
        {
            if (localContactPoint.y > bikeCenter.y)
            {
                if (collision.impulse.magnitude / bikeRigidbody.mass > impactThreshold)
                    DislodgePlayer();
            }
        }
        else
        {
            if (collision.impulse.magnitude / bikeRigidbody.mass > impactThreshold)
                DislodgePlayer();
        }
    }

    public void DislodgePlayer(bool isWheelie = false, bool isStoppie = false)
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
            if (isWheelie)
                rb.velocity = bikeVelocity*2;
            else
                rb.velocity = bikeVelocity;
            rb.angularVelocity = bikeAngularVelocity;
        }

        foreach (Rigidbody rb in characterRagdollRigidbodies)
        {
            if (!isWheelie)
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

        // Deactivate the original bike
        gameObject.SetActive(false);

        // Invoke the event
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
