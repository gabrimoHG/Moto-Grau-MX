using UnityEngine;
using Cinemachine;
using HistoricGamesUtils;
using Unity.VisualScripting;
using Unity.Mathematics;
using TMPro.Examples;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

namespace ArcadeBP_Pro
{

    public class CameraController : MonoBehaviour
    {
        GameManager gameManager;
        [Tooltip("Array of Cinemachine virtual cameras.")]
        public CinemachineVirtualCamera[] cameras;

        public ArcadeBikeControllerPro bikeController;

        [Tooltip("Speed threshold above which the camera shake effect starts.")]
        public float speedThresholdForShake = 50f;

        [Tooltip("Amplitude of the camera shake effect.")]
        public float shakeAmplitude = 1.2f;

        [Tooltip("Frequency of the camera shake effect.")]
        public float shakeFrequency = 2.0f;

        [Tooltip("Key to switch between different cameras.")]
        public KeyCode switchCameraKey = KeyCode.C;

        [Tooltip("Minimum camera FOV value.")]
        public float minFOV = 60f;

        [Tooltip("Maximum camera FOV value.")]
        public float maxFOV = 80f;

        [Tooltip("Smoothing factor for the camera FOV value.")]
        public float FOV_smoother = 5f;

        [SerializeField] float firstCamReverseZOffset;
        [SerializeField] float firstCamStandardZOffset;
        [SerializeField] float firstCamOnWheelieZOffset;
        [SerializeField] float secondCamReverseZOffset;
        [SerializeField] float secondCamStandardZOffset;
        [SerializeField] float thirdCamReverseZOffset;
        [SerializeField] float thirdCamStandardZOffset;
        float fpsXOffset;
        [SerializeField] float perspectiveChangeDuration;
        [SerializeField] float defaultFPSYOffset;
        [SerializeField] float onWheelieFPSYOffset;
        [SerializeField] bool isFirstPersonCamDynamic;
        public bool IsFirstPersonCamDynamic { get { return isFirstPersonCamDynamic; } }
        public void SetIsFirstPersonCamDynamic(bool isFirstPersonCamDynamic) =>
            this.isFirstPersonCamDynamic = isFirstPersonCamDynamic;
        float currentFPSYOffset;
        private CinemachineBasicMultiChannelPerlin[] cameraNoise;
        public int currentCameraIndex { private set; get; }
        public bool isFirstPerson { private set; get; }
        CinemachineTransposer transposer;
        private bool isShaking = false;
        bool isManuallyMoving, isRotated;
        BikeAdditionalPhysics bikeAdditionalPhysics;
        Transform firstPersonFollowTarget;
        
        Transform[] thirdPersonFollowTargets;
        Transform[] thirdPersonLookAtTargets;
        WheelieRating wheelieRating;
        float currentVelocity;

        
        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            transposer = cameras[currentCameraIndex].
                    GetCinemachineComponent<CinemachineTransposer>();
            wheelieRating = FindObjectOfType<WheelieRating>();
            smoothFOV = 60f;
            //defaultFPSYOffset = 10;
            //onWheelieFPSYOffset = -20;
            //transform.name = "CameraController_" + RemovePrefix(bikeController.transform.name, "ABP_Pro");
        }
        
        private void OnDestroy()
        {
            gameManager.onPlayerBikeChange -= UpdateBikeReferences;//Revisado!
            if(bikeAdditionalPhysics!=null)
                bikeAdditionalPhysics.onDislodged -= SetCameraTargetToRagdoll;
        }
        
        void Start()
        {
            gameManager.onPlayerBikeChange += UpdateBikeReferences;//Revisado!
            Camera.main.GetComponent<PostProcessLayer>().enabled= false;
            InitialDataSet();
        }
        void UpdateBikeReferences(ArcadeBikeControllerPro bikeController)
        {
            this.bikeController = bikeController;
            //Debug.Log("Bike: " + transform.parent);
            StopAllCoroutines();
            InitialDataSet();
            //Debug.Log("Data updated!");
        }


        void InitialDataSet()
        {
            cameraNoise = new CinemachineBasicMultiChannelPerlin[cameras.Length];
            gameManager.onBikeReset += ResetCameraTarget;
            thirdPersonFollowTargets = new Transform[cameras.Length - 1];
            thirdPersonLookAtTargets = new Transform[cameras.Length - 1];
            speedThresholdForShake = bikeController.bikeSettings.maxSpeed * .75f;


            gameManager.SetCameraController(this);
            bikeAdditionalPhysics = gameManager.GetCurrentBikePhysicsController()
                .GetComponent<BikeAdditionalPhysics>();//Revisado!
            bikeAdditionalPhysics.onDislodged += SetCameraTargetToRagdoll;
            UIManager.instance.cameraAlternation.UpdateSubscription(this);
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].gameObject.SetActive(i == currentCameraIndex);
                cameraNoise[i] = cameras[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                if (i < cameras.Length - 1)
                {
                    thirdPersonFollowTargets[i] = cameras[i].Follow;
                    thirdPersonLookAtTargets[i] = cameras[i].LookAt;
                }
                else
                {
                    firstPersonFollowTarget = cameras[i].Follow;
                    firstPersonFollowTarget = cameras[i].LookAt;
                }
            }
            var cameraIndex = gameManager.bikeSpawner.lastCameraIndex;
            SwitchCamera(cameraIndex);
        }
        void Update()
        {
            if (Input.GetKeyDown(switchCameraKey))
                SwitchCamera();
            SetReversePerspective();
            float bikeSpeed = bikeController.localBikeVelocity.magnitude;
            if (bikeSpeed > speedThresholdForShake && bikeController.isActiveAndEnabled)
            {
                UpdateShake(bikeSpeed);
            }
            else if (isShaking)
            {
                StopShake();
            }
            FPSRotationAdjustment();
            OnWheelieAdjustments();
            //UpdateFOV(bikeSpeed);
        }

        void OnWheelieAdjustments()
        {
            if (currentCameraIndex == 0) {

                transposer = cameras[currentCameraIndex].
                    GetCinemachineComponent<CinemachineTransposer>();
                var initialOffset = transposer.m_FollowOffset;
                var targetOffset = bikeController.isDoingWheelie ? 
                    new Vector3(initialOffset.x, initialOffset.y, firstCamOnWheelieZOffset)
                    : new Vector3(initialOffset.x, initialOffset.y, -1f);
                transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset,
                    targetOffset, Time.deltaTime);
            }
            else if (currentCameraIndex == 2)
            {
                
                transposer = cameras[currentCameraIndex].
                    GetCinemachineComponent<CinemachineTransposer>();
                var initialOffset = transposer.m_FollowOffset;
                var wheelieAngle = bikeController.currentWheelieAngle;
                var angleLimit = bikeController.bikeSettings.wheelieAngleLimit;


                float normalizedSpeed = Mathf.Clamp01(wheelieAngle / angleLimit);
                float targetXOffset = Mathf.Lerp(0, 0.4f, normalizedSpeed);
                fpsXOffset = Mathf.SmoothDamp(fpsXOffset, targetXOffset, ref currentVelocity, .25f);
                Vector3 followOffset = transposer.m_FollowOffset;
                followOffset.x = fpsXOffset;
                transposer.m_FollowOffset = followOffset;
            }
        }
        void FPSRotationAdjustment()
        {
            var fpsCamera = cameras[currentCameraIndex];
            var composer = fpsCamera.GetCinemachineComponent<CinemachineComposer>();
            var wheelieAngle = 360 - wheelieRating.currentAngle;
            if (isFirstPerson && wheelieAngle > 10 && bikeController.isDoingWheelie &&
                isFirstPersonCamDynamic)
            {
                currentFPSYOffset = Mathf.Lerp(currentFPSYOffset,
                    onWheelieFPSYOffset, Time.deltaTime);
                Vector3 offset = composer.m_TrackedObjectOffset;
                offset.y = currentFPSYOffset;
                composer.m_TrackedObjectOffset = offset;
            }
            else if (isFirstPerson)
            {
                currentFPSYOffset = Mathf.Lerp(currentFPSYOffset,
                    defaultFPSYOffset, Time.deltaTime);
                Vector3 offset = composer.m_TrackedObjectOffset;
                offset.y = currentFPSYOffset;
                offset.x = fpsXOffset;
                composer.m_TrackedObjectOffset = offset;
            }
        }
        public void SwitchCamera()
        {
            cameras[currentCameraIndex].gameObject.SetActive(false);
            if (currentCameraIndex < cameras.Length - 1)
                currentCameraIndex++;
            else
                currentCameraIndex = 0;

            if (currentCameraIndex == 2)
            {
                isFirstPerson = true;
                transposer = cameras[currentCameraIndex].
                    GetCinemachineComponent<CinemachineTransposer>();
                Vector3 followOffset = transposer.m_FollowOffset;
                Vector3 targetPosition = transposer.FollowTarget.position + 
                    transposer.LookAtTarget.rotation * followOffset;
                cameras[currentCameraIndex].transform.position = targetPosition;
            }
            else
                isFirstPerson = false;
            cameras[currentCameraIndex].gameObject.SetActive(true);
            isManuallyMoving = false;
            //Debug.Log("False by switch!");
            StopShake();
        }
        void SwitchCamera(int specificIndex)
        {
            cameras[currentCameraIndex].gameObject.SetActive(false);

            if (specificIndex == 2)
            {
                isFirstPerson = true;
                transposer = cameras[specificIndex].
                    GetCinemachineComponent<CinemachineTransposer>();
                Vector3 followOffset = transposer.m_FollowOffset;
                Vector3 targetPosition = transposer.FollowTarget.position +
                    transposer.LookAtTarget.rotation * followOffset;
                cameras[specificIndex].transform.position = targetPosition;
            }
            else
                isFirstPerson = false;
            cameras[specificIndex].gameObject.SetActive(true);
            isManuallyMoving = false;
            currentCameraIndex = specificIndex;
            //Debug.Log("False by switch!");
            StopShake();
        }
        void UpdateShake(float bikeSpeed)
        {
            isShaking = true;
            if (cameraNoise[currentCameraIndex] != null)
            {
                float t = Mathf.InverseLerp(speedThresholdForShake,
                    bikeController.bikeSettings.maxSpeed, bikeSpeed);


                if (isFirstPerson)
                {
                    cameraNoise[currentCameraIndex].m_AmplitudeGain = Mathf.Lerp(0, shakeAmplitude / 2, t);
                    cameraNoise[currentCameraIndex].m_FrequencyGain = Mathf.Lerp(0, shakeFrequency / 2, t);

                }
                else
                {
                    cameraNoise[currentCameraIndex].m_AmplitudeGain = Mathf.Lerp(0, shakeAmplitude, t);
                    cameraNoise[currentCameraIndex].m_FrequencyGain = Mathf.Lerp(0, shakeFrequency, t);
                }
            }
        }

        void StopShake()
        {
            isShaking = false;

            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameraNoise[i] != null)
                {
                    cameraNoise[i].m_AmplitudeGain = 0f;
                    cameraNoise[i].m_FrequencyGain = 0f;
                }
            }
        }


        float smoothFOV = 60f;
        void UpdateFOV(float bikeSpeed)
        {
            float t = Mathf.InverseLerp(0, bikeController.bikeSettings.maxSpeed, bikeSpeed);


            float newFOV = 0;
            if(isFirstPerson)
                newFOV = Mathf.Lerp(minFOV, maxFOV*.85f, t);
            else
                newFOV = Mathf.Lerp(minFOV, maxFOV, t);

            smoothFOV = Mathf.Lerp(smoothFOV, newFOV, Time.deltaTime * FOV_smoother);

            cameras[currentCameraIndex].m_Lens.FieldOfView = smoothFOV;
        }


        public void SetCameratarget(Transform followTarget, Transform lookAtTarget)
        {
            foreach (var camera in cameras)
            {
                camera.LookAt = lookAtTarget;
                camera.Follow = followTarget;
            }

            StopShake();
        }

        public void ResetCameraTarget()
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (i < cameras.Length - 1)
                {
                    cameras[i].Follow = thirdPersonFollowTargets[i];
                    cameras[i].LookAt = thirdPersonLookAtTargets[i];
                    /*var transposer = cameras[currentCameraIndex].
                        GetCinemachineComponent<CinemachineTransposer>();
                    var newOffset = new Vector3(transposer.m_FollowOffset.x,
                        transposer.m_FollowOffset.y, transposer.m_FollowOffset.z - 1);
                    transposer.m_FollowOffset = newOffset;*/
                }
                else
                {
                    cameras[i].Follow = firstPersonFollowTarget;
                    cameras[i].LookAt = firstPersonFollowTarget;
                }
            }

            StopShake();
        }


        void SetReversePerspective()
        {
            var reversePos = Vector3.zero;
            var target = transform;
            if (firstPersonFollowTarget == null)
                return;
            if (currentCameraIndex <= 1)
            {
                if(thirdPersonFollowTargets[currentCameraIndex] == null)
                    return;
                target = thirdPersonFollowTargets[currentCameraIndex];
            }
            else
                target = firstPersonFollowTarget;

            if (bikeController.bikeInput.reverse > 0 &&
                bikeController.isReversing && !isManuallyMoving)
            {
                if (currentCameraIndex == 0)
                    reversePos = new Vector3(0, 0, firstCamReverseZOffset);
                else if (currentCameraIndex == 1)
                {
                    var standardY = thirdPersonFollowTargets[currentCameraIndex].localPosition.y;
                    reversePos = new Vector3(0, standardY, secondCamReverseZOffset);
                }
                else
                {
                    var standardY = firstPersonFollowTarget.localPosition.y;
                    reversePos = new Vector3(0, standardY, thirdCamReverseZOffset);
                    if (!isRotated)
                    {
                        Quaternion firstPersonRotation = Quaternion.Euler(
                            target.localRotation.eulerAngles.x,150, target.localRotation.eulerAngles.z);
                        StartCoroutine(InterpolationUtilities.RotateToTargetLinearly(
                            target, target.localRotation, firstPersonRotation, .4f, null, true));
                        isRotated = true;
                    }
                }
                
                StartCoroutine(InterpolationUtilities.MoveToTargetLinearly(target, target.localPosition,
                    reversePos, perspectiveChangeDuration, ResetIsManuallyMoving, true));
                isManuallyMoving = true;
            }
            else if(!isManuallyMoving)
            {
                if (currentCameraIndex == 0)
                    reversePos = new Vector3(0, 0, firstCamStandardZOffset);
                else if (currentCameraIndex == 1)
                {
                    var standardY = thirdPersonFollowTargets[currentCameraIndex].localPosition.y;
                    reversePos = new Vector3(0, standardY, secondCamStandardZOffset);
                }
                else
                {
                    var standardY = firstPersonFollowTarget.localPosition.y;
                    reversePos = new Vector3(0, standardY, thirdCamStandardZOffset);
                    if (isRotated)
                    {
                        Quaternion firstPersonRotation = Quaternion.Euler(
                            target.localRotation.eulerAngles.x,
                            0, target.localRotation.eulerAngles.z);
                        StartCoroutine(InterpolationUtilities.RotateToTargetLinearly(
                            target, target.rotation, firstPersonRotation, .4f, null, true));
                        isRotated = false;
                    }
                }
                StartCoroutine(InterpolationUtilities.MoveToTargetLinearly(target, target.localPosition,
                    reversePos, perspectiveChangeDuration, ResetIsManuallyMoving, true));
                isManuallyMoving = true;
            }
        }
        void ResetIsManuallyMoving() 
        { 
            isManuallyMoving = !isManuallyMoving;
            //Debug.Log("False by movement finish!");
        }

        public void SetCameraTargetToRagdoll()
        {
            transform.parent = null;
            var targetTransform = transform;
            var ragdollInstance = bikeAdditionalPhysics.characterRagdollInstance;
            if (currentCameraIndex != 2)
            {
                targetTransform = ragdollInstance.GetComponent<Animator>().
                    GetBoneTransform(HumanBodyBones.UpperChest);
                /*var transposer = cameras[currentCameraIndex].
                    GetCinemachineComponent<CinemachineTransposer>();
                var newOffset = new Vector3(transposer.m_FollowOffset.x,
                    transposer.m_FollowOffset.y, transposer.m_FollowOffset.z + 1);
                transposer.m_FollowOffset = newOffset;*/
                StartCoroutine(WaitAndCleanTarget(transposer));
            }
            else
            {
                targetTransform = ragdollInstance.GetComponent<Animator>().
                    GetBoneTransform(HumanBodyBones.Head);
            }
            SetCameratarget(targetTransform, targetTransform);
        }

        IEnumerator WaitAndCleanTarget(CinemachineTransposer transposer)
        {
            yield return new WaitForSeconds(1.5f);
            cameras[currentCameraIndex].Follow = null;
            cameras[currentCameraIndex].LookAt = null;
            transform.parent = bikeController.transform;
        }

        private string RemovePrefix(string originalName, string prefix)
        {
            if (originalName.StartsWith(prefix))
            {
                return originalName.Substring(prefix.Length);
            }
            return originalName;
        }

    }

}
