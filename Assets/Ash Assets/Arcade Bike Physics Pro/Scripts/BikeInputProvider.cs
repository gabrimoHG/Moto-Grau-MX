using UnityEngine;

namespace ArcadeBP_Pro
{
    public class BikeInputProvider : MonoBehaviour
    {
        ArcadeBikeControllerPro bikeController;

        [Header("Keyboard Inputs")]

        [Tooltip("The key to accelerate the bike. Default is usually `KeyCode.W`.")]
        public KeyCode AccelerateKey;

        [Tooltip("The key to reverse the bike. Default is `KeyCode.S`.")]
        public KeyCode ReverseKey;

        [Tooltip("The key to apply the hand brake. Default is `KeyCode.Space`.")]
        public KeyCode HandBrakeKey;

        [Tooltip("The key to steer the bike left. Default is `KeyCode.A`.")]
        public KeyCode SteeringLeftKey;

        [Tooltip("The key to steer the bike right. Default is `KeyCode.D`.")]
        public KeyCode SteeringRightKey;

        [Tooltip("The key to perform a wheelie. Default is usually `KeyCode.LeftShift`.")]
        public KeyCode WheelieKey;

        //[Tooltip("The key to perform a stoppie. Default is usually `KeyCode.LeftShift`.")]
        public KeyCode StoppieKey;

        [Header("Mobile Inputs")]

        [Tooltip("UI button for accelerating the bike.")]
        public UiButton_ABP_Pro accelerateButton;

        [Tooltip("UI button for reversing the bike.")]
        public UiButton_ABP_Pro reverseButton;

        [Tooltip("UI button for applying the handbrake.")]
        public UiButton_ABP_Pro handBrakeButton;

        [Tooltip("UI button for steering the bike to the left.")]
        //public UiButton_ABP_Pro SteeringLeftButton;

        DirectionalHandlerController directionalHandlerController;
        //[Tooltip("UI button for steering the bike to the right.")]
        //public UiButton_ABP_Pro SteeringRightButton;

        //[Tooltip("UI button for performing a wheelie.")]
        public UiButton_ABP_Pro WheelieButton;

        // inputs to provide
        private float accelerate, reverse, handbrake,
            steering, wheelie, stoppie;

        [ContextMenu("Set Default Inputs")]
        private void setDefaultInput()
        {
            AccelerateKey = KeyCode.W;
            ReverseKey = KeyCode.S;
            HandBrakeKey = KeyCode.Space;
            SteeringLeftKey = KeyCode.A;
            SteeringRightKey = KeyCode.D;
            WheelieKey = KeyCode.LeftShift;
        }
        private void Awake()
        {
            bikeController = GetComponent<ArcadeBikeControllerPro>();
        }
        private void Start()
        {
            directionalHandlerController = UIManager.instance.directionalController;
            accelerateButton = UIManager.instance.accelerationButton;
            reverseButton = UIManager.instance.breakButton;
            handBrakeButton = UIManager.instance.handbreakButton;
        }
        private void Update()
        {
            // set inputs
            SetPlayerInput();
        }

        private void SetPlayerInput()
        {
            // get inputs
            accelerate = (Input.GetKey(AccelerateKey) || accelerateButton?.isPressed == true) ? 1f : 0f;
            reverse = (Input.GetKey(ReverseKey) || reverseButton?.isPressed == true) ? 1f : 0f;
            handbrake = (Input.GetKey(HandBrakeKey) || handBrakeButton?.isPressed == true) ? 1f : 0f;
            float verticalDeadZone = .3f;
            float horizontalDeadZone = .5f;
            //Debug.Log("Horizontal " + directionalHandlerController.direction.x);
            //Debug.Log("Vertical "+directionalHandlerController.direction.y);

            steering = Input.GetKey(SteeringLeftKey) ? -1f : 0f;
            if (steering == 0)
            {
                steering = directionalHandlerController.direction.x < 0 &&
                directionalHandlerController.direction.y <= verticalDeadZone &&
                directionalHandlerController.direction.y >= -verticalDeadZone ?
                directionalHandlerController.direction.x : 0f;
            }

            if (steering == 0)
            {
                steering = Input.GetKey(SteeringRightKey) ? 1f : 0f;

                if (steering == 0)
                {
                    steering = directionalHandlerController.direction.x > 0 &&
                    directionalHandlerController.direction.y <= verticalDeadZone &&
                    directionalHandlerController.direction.y >= -verticalDeadZone ?
                    directionalHandlerController.direction.x : 0f;
                }
            }

            wheelie = Input.GetKey(WheelieKey) ? -1f : 0f;
            if (wheelie == 0)
            {
                wheelie = directionalHandlerController.direction.y < 0 &&
                directionalHandlerController.direction.x <= horizontalDeadZone &&
                directionalHandlerController.direction.x >= -horizontalDeadZone ?
                directionalHandlerController.direction.y : 0f;
            }

            stoppie = (Input.GetKey(StoppieKey) || directionalHandlerController.verticalAxis > 0) ? 1f : 0f;
            if (stoppie == 0)
            {
                stoppie = directionalHandlerController.direction.y > 0 &&
                directionalHandlerController.direction.x <= horizontalDeadZone &&
                directionalHandlerController.direction.x >= -horizontalDeadZone ?
                directionalHandlerController.direction.y : 0f;
            }
            // Note : You can also use your custom inputs above to provide inputs to the bike controller
            // provide inputs to the bike controller
            bikeController.ProvideInput(accelerate, reverse, handbrake, steering, wheelie);
        }

    }
}
