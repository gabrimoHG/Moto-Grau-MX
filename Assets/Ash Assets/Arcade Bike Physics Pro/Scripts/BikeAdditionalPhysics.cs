using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using ArcadeBP_Pro;
using HistoricGamesUtils;
using UnityEngine.Rendering;
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

    public event Action onDislodged = delegate { };

    private Rigidbody bikeRigidbody;
    bool isRagdollActivated = false;
    bool collisionCooldown = false;
    [SerializeField] float cooldownDuration;
    [SerializeField] AudioSource crashAudioSource;
    [SerializeField] AudioClip smallCrashClip;
    private GameObject bikeRagdollInstance;
    float damage;

    public Transform GetRagdollBikeTransform() { return bikeRagdollInstance.transform; }
    public GameObject characterRagdollInstance { get; private set; }
    private Collider bikeCollider;

    private void Awake()
    {
        bikeRigidbody = GetComponent<Rigidbody>();
#if UNITY_EDITOR_64
        if (cooldownDuration == 0)
            Debug.LogWarning("Collision cooldown is 0!");
#endif
    }
    void Start()
    {
        bikeCollider = bikeController.bikeReferences.collider;
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided with " + collision.gameObject);
        if (isRagdollActivated || collisionCooldown || collision.gameObject.layer == 6)
        {
            crashImpact = 0;
            //collisionCooldown = false;
            //Debug.Log("Cooldown!");
            return;
        }

        ContactPoint[] contacts = new ContactPoint[collision.contactCount];
        collision.GetContacts(contacts);
        var backBodyCollider = bikeCollider.transform.GetChild(2).GetComponent<CapsuleCollider>();
        foreach (ContactPoint contact in contacts)
        {
            Collider colliderOnBike = contact.thisCollider;

            if (colliderOnBike == backBodyCollider)
                return;
        }

        crashImpact = collision.impulse.magnitude / bikeRigidbody.mass;
        var wheelieTransform = bikeController.bikeReferences.wheelieTransform;
        float clampedImpact = 0;
        float crashOutputDegree = 0;
        //Debug.Log("Impact " + crashImpact);
        //Debug.Break();
        if (!bikeController.isDoingWheelie &&
            !bikeController.isDoingStoppie)
        {
            clampedImpact = Mathf.Clamp(crashImpact, 0, impactCeiling);
            damage = Mathf.Lerp(0, 100, clampedImpact / impactCeiling);
            crashOutputDegree = Mathf.Lerp(0, 35, (crashImpact - 2) / (impactThresholdToFall - 2));
            wheelieTransform.localRotation = Quaternion.Euler(crashOutputDegree, 0, 0);
        }
        else if (bikeController.isDoingWheelie)
        {
            clampedImpact = Mathf.Clamp(crashImpact, 0, impactCeiling / 2.5f);
            damage = Mathf.Lerp(0, 20, clampedImpact / (impactCeiling / 2.5f));

            crashOutputDegree = 0;


            if (bikeController.wheelieAngle <= 15)
            {
                crashOutputDegree = Mathf.Lerp(0, bikeController.wheelieAngle - 15, crashImpact);
            }
            else
            {
                crashOutputDegree = Mathf.Lerp(0,
                    (bikeController.wheelieAngle + 30) * -1, crashImpact);
            }
            StartCoroutine(InterpolationUtilities.RotateInXLinearly(wheelieTransform,
                crashOutputDegree, .25f, OutcomeAfterCrashOnWheelie, true));
        }
        else
        {
            //Debug.Log("Revisar impacto durante stoppie!");
            clampedImpact = Mathf.Clamp(crashImpact, 0, impactCeiling);
            damage = Mathf.Lerp(0, 20, clampedImpact / (impactCeiling / 2));
            crashOutputDegree = 0;
            crashOutputDegree = Mathf.Lerp(0, -20, crashImpact);
            StartCoroutine(InterpolationUtilities.RotateInXLinearly(wheelieTransform,
                crashOutputDegree, .25f, null, true));
        }
        //Debug.Log("Damage " + damage);
        bikeController.DecreaseCondition((int)damage);
        damage = 0;
        StartCoroutine(CollisionCooldownRoutine());
        //Debug.Log(crashImpact);
        var crashVolume = crashImpact / impactThresholdToFall;
        if (!crashAudioSource.isPlaying)
        {
            crashAudioSource.clip = smallCrashClip;
            crashAudioSource.volume = crashVolume;
            crashAudioSource.Play();
        }
        if (crashImpact > impactThresholdToFall)
            DislodgePlayer();
    }
    public void DislodgePlayer(bool isDoingWheelie = false, bool isDoingStoppie = false)
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
                rb.velocity = bikeVelocity * 2;
            else if(isDoingStoppie)
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
            else if(isDoingStoppie)
            {
                rb.velocity = -bikeVelocity;
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
        else if(isDoingStoppie)
        {
            damage = Mathf.Lerp(0, 100, bikeController.speedKmph /
                bikeController.stoppieAngle);
        }


        //Debug.Log("Damage " + damage);
        bikeController.DecreaseCondition((int)damage);
        gameObject.SetActive(false);
        collisionCooldown = false;
        onDislodged();
    }

    void OutcomeAfterCrashOnWheelie()
    {
        var wheelieTransform = bikeController.bikeReferences.wheelieTransform;
        if (wheelieTransform.localRotation.eulerAngles.x <
            (360 - bikeController.bikeSettings.wheelieAngleLimit))
            DislodgePlayer(true, false);
        //Debug.Log("Wheelie rotation "+wheelieTransform.localRotation.eulerAngles.x);
        //Debug.Log("Wheelie limit " + bikeController.bikeSettings.wheelieAngleLimit);
    }
    IEnumerator CollisionCooldownRoutine()
    {
        collisionCooldown = true;  // Enable cooldown
        yield return new WaitForSeconds(cooldownDuration);  // Wait for a few ticks or seconds
        collisionCooldown = false;  // Disable cooldown after the delay
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

    public void ForceActivateRagdoll()
    {
        if (!isRagdollActivated)
        {
            DislodgePlayer();
        }
    }

}
