using ArcadeBP_Pro;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public static GameManager instance { get; private set; }
    public BikeSpawner bikeSpawner { get; private set; }
    
    Vector3 spawnPosition;
    Quaternion spawnRotation;
    ArcadeBikeControllerPro bikePhysicsController;
    public event Action<ArcadeBikeControllerPro> onPlayerBikeChange = delegate { };
    UIManager uiManager;
    public int targetFrameRate = 60;
    public int secondaryTargetFrameRate = 30;
    public float calibrationDuration = 5f;
    private int currentQualityLevel;
    private bool isCalibrating = true;
    private float timeElapsed = 0;
    private float fpsAccumulator = 0;
    private int frameCount = 0;
    public ArcadeBikeControllerPro GetCurrentBikePhysicsController() 
    {
        return bikePhysicsController; 
    }
    public void RaiseOnPlayerBikeChange(ArcadeBikeControllerPro bikePhysicsController)
    {
        this.bikePhysicsController = bikePhysicsController;
        this.bikePhysicsController.SetConsumablesStatus();
        bikeAdditionalPhysics = bikePhysicsController.
            GetComponent<BikeAdditionalPhysics>();
        bikeAdditionalPhysics.onDislodged += UpdateSpawnTransform;
        onPlayerBikeChange(this.bikePhysicsController);
    }


    public WheelieRating wheelieRating { get; private set; }

    static bool isLocked;
    public BikeAdditionalPhysics bikeAdditionalPhysics { get; private set; }
    public CameraController cameraController { get; private set; }
    public BikesController bikesController { get; private set; }
    public RidersController ridersController { get; private set; }
    public void SetCameraController(CameraController cameraController) =>
                this.cameraController = cameraController;
    public event Action onBikeReset = delegate { };
    public bool isPaused { get; private set; }
    private void Awake()
    {
        //SetSingletonInstance();
        wheelieRating = GetComponent<WheelieRating>();
        bikeSpawner = FindObjectOfType<BikeSpawner>();
        bikesController = GetComponent<BikesController>();
        ridersController = GetComponent<RidersController>();
        LoadSelectedBike();
        LoadSelectedRider();
        //LoadSelectedBike();
        //LoadSelectedRider();
        /*if (bikePhysicsController != null)
            Debug.Log("Physics controller is valid!");*/
        //Debug.Log($"{name} - Game Manager Awake called at frame {Time.frameCount}");
    }
    private void Start()
    {
        /*if (PlayerPrefs.HasKey("GraphicQuality"))
            QualitySettings.SetQualityLevel((int)PlayerPrefs.GetFloat("GraphicQuality"));*/
        if (!PlayerPrefs.HasKey("GraphicQuality"))
        {
            var graphicsQualitySlider = UIManager.instance.graphicsQualitySlider;
            QualitySettings.SetQualityLevel((int)graphicsQualitySlider.GetComponent<Slider>().value);
        }
        currentQualityLevel = QualitySettings.GetQualityLevel();
        StartCalibration();
    }

    private void StartCalibration()
    {
        isCalibrating = true;
        timeElapsed = 0;
        fpsAccumulator = 0;
        frameCount = 0;
    }

    private void Update()
    {
        if (!isCalibrating) return;

        // Accumulate FPS data
        fpsAccumulator += (1f / Time.unscaledDeltaTime);
        frameCount++;
        timeElapsed += Time.unscaledDeltaTime;

        if (timeElapsed >= calibrationDuration)
        {
            float averageFPS = fpsAccumulator / frameCount;
            EvaluatePerformance(averageFPS);
        }
    }

    private void EvaluatePerformance(float averageFPS)
    {
        if (averageFPS >= targetFrameRate)
        {
            //Debug.Log($"Target FPS {targetFrameRate} reached. Average FPS: {averageFPS}");
            isCalibrating = false;
        }
        else if (currentQualityLevel > 0)
        {
            currentQualityLevel--;
            QualitySettings.SetQualityLevel(currentQualityLevel, true);
            //Debug.Log($"Lowering quality to level {currentQualityLevel}. Recalibrating...");
            StartCalibration();
        }
        else if (averageFPS < targetFrameRate && targetFrameRate == 60)
        {
            //Debug.Log($"60 FPS unreachable. Adjusting target to 30 FPS...");
            targetFrameRate = secondaryTargetFrameRate;
            StartCalibration();
        }
        else
        {
            //Debug.Log($"Minimum quality level reached. Average FPS: {averageFPS}");
            isCalibrating = false;
        }
    }

    void LoadSelectedBike()
    {
        var selectedBikeID = BikeSelectionScreen.currentBikeID;
        bikePhysicsController = bikesController.GetBike(selectedBikeID);

        var selectedBodyColor = MainMenuManager.instance.paintPartsScreen.bodyColor;
        var selectedRimsColor = MainMenuManager.instance.paintPartsScreen.rimsColor;
        var spawnPoint = bikeSpawner.SortInitialSpawnPoint();
        spawnPosition = spawnPoint.Key;
        spawnRotation = Quaternion.Euler(0, spawnPoint.Value, 0);
        var bikeInstance = Instantiate(bikePhysicsController, spawnPosition, spawnRotation);
        bikePhysicsController = bikeInstance;
        bikePhysicsController.bikeReferences.cameraController.gameObject.SetActive(true);
        bikePhysicsController.SetConsumablesStatus();
        bikePhysicsController.GetComponent<BikeComponent>().SetBodyColor(selectedBodyColor);
        bikePhysicsController.GetComponent<BikeComponent>().SetRimsColor(selectedRimsColor);
        cameraController = bikePhysicsController.bikeReferences.cameraController;
        bikeAdditionalPhysics = bikePhysicsController.GetComponent<BikeAdditionalPhysics>();
        bikeAdditionalPhysics.onDislodged += UpdateSpawnTransform;
    }

    void LoadSelectedRider()
    {
        var selectedRiderID = MainMenuManager.instance.riderSelectionScreen.currentRiderID;
        var bikerController = ridersController.GetRider(selectedRiderID);
        var parent = bikePhysicsController.bikeReferences.bikeModel;
        var instance = Instantiate(bikerController, spawnPosition, spawnRotation, parent);
    }

    public void SetGraphicQuality(float qualityIndex)
    {
        if (qualityIndex < 3)
            QualitySettings.SetQualityLevel((int)qualityIndex, false);
        else
            QualitySettings.SetQualityLevel((int)qualityIndex, true);
    }

    void UpdateSpawnTransform()
    {
        spawnPosition = bikeAdditionalPhysics.GetRagdollBikeTransform().position;
    }
    /*void SetSingletonInstance()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }*/
    public void SetPauseState(bool isPaused)
    {
        if (isPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
        this.isPaused = isPaused;
    }

    public void SetDestruction(GameObject expendableObject, float lifeCycle=60)=>
        Destroy(expendableObject, lifeCycle);
    public void ResetBike()
    {

        bikePhysicsController.bikeReferences.bikeRb.velocity = Vector3.zero;
        bikePhysicsController.bikeReferences.bikeRb.angularVelocity = Vector3.zero;
        bikePhysicsController.bikeReferences.rotator.transform.localRotation = Quaternion.identity;
        bikePhysicsController.bikeReferences.wheelieTransform.localRotation = Quaternion.identity;
        bikePhysicsController.SetIsDoingStoppie(false);
        bikePhysicsController.SetIsDoingWheelie(false);
        bikePhysicsController.transform.position = spawnPosition;

        bikeAdditionalPhysics.CleanRagdolls();
        bikePhysicsController.gameObject.SetActive(true);
        bikePhysicsController.SetCanAccelerate(true);
        bikePhysicsController.bikeAudio.engineSound.pitch = bikePhysicsController.bikeAudio.minPitch;
        bikePhysicsController.bikeReferences.cameraController.gameObject.SetActive(true);

        onBikeReset();
    }



}
