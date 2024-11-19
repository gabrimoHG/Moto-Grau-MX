using ArcadeBP_Pro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public BikeSpawner bikeSpawner { get; private set; }
    [SerializeField]ArcadeBikeControllerPro currentBikeController;
    [SerializeField] Vector3 spawnPosition;
    Quaternion spawnRotation;
    public event Action<ArcadeBikeControllerPro> onPlayerBikeChange = delegate { };
    public WheelieRating wheelieRating { get; private set; }
    public ArcadeBikeControllerPro GetCurrentBikeController() { return currentBikeController; }
    public void RaiseOnPlayerBikeChange(ArcadeBikeControllerPro arcadeBikeController)
    {
        currentBikeController = arcadeBikeController;
        arcadeBikeController.SetConsumablesStatus();
        onPlayerBikeChange(arcadeBikeController);
    }
    public BikeAdditionalPhysics bikeAdditionalPhysics { get; private set; }
    public CameraController cameraController { get; private set; }
    public void SetCameraController(CameraController cameraController) =>
                this.cameraController = cameraController;
    public event Action onBikeReset = delegate { };
    public bool isPaused { get; private set; }
    private void Awake()
    {
        SetSingletonInstance();
        InitialSet();
    }
    void InitialSet()
    {
        bikeSpawner = FindObjectOfType<BikeSpawner>();
        spawnRotation = Quaternion.Euler(0, -90, 0);
        var bikeInstance = Instantiate(currentBikeController, spawnPosition, spawnRotation);
        currentBikeController = bikeInstance;
        currentBikeController.bikeReferences.cameraController.gameObject.SetActive(true);
        currentBikeController.SetConsumablesStatus();
        cameraController = bikeInstance.bikeReferences.cameraController;
        bikeAdditionalPhysics = currentBikeController.GetComponent<BikeAdditionalPhysics>();
        bikeAdditionalPhysics.onDislodged += UpdateSpawnTransform;
        wheelieRating = GetComponent<WheelieRating>();
        
    }
    void UpdateSpawnTransform()
    {
        spawnPosition = bikeAdditionalPhysics.GetRagdollBikeTransform().position;
    }
    void SetSingletonInstance()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

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

        currentBikeController.bikeReferences.bikeRb.velocity = Vector3.zero;
        currentBikeController.bikeReferences.bikeRb.angularVelocity = Vector3.zero;
        currentBikeController.bikeReferences.rotator.transform.localRotation = Quaternion.identity;
        currentBikeController.bikeReferences.wheelieTransform.localRotation = Quaternion.identity;
        currentBikeController.SetIsDoingStoppie(false);
        currentBikeController.SetIsDoingWheelie(false);
        currentBikeController.transform.position = spawnPosition;

        bikeAdditionalPhysics.CleanRagdolls();
        currentBikeController.gameObject.SetActive(true);
        currentBikeController.SetCanAccelerate(true);
        currentBikeController.bikeAudio.engineSound.pitch = currentBikeController.bikeAudio.minPitch;
        currentBikeController.bikeReferences.cameraController.gameObject.SetActive(true);

        onBikeReset();
    }



}
