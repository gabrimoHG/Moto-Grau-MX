using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HistoricGamesUtils;
using System.Linq.Expressions;
using ArcadeBP_Pro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public GameManager gameManager { get; private set; }
    public DirectionalHandlerController directionalController { private set; get; }
    public Transform baseContextualUI { private set; get; }
    public UiButton_ABP_Pro accelerationButton { private set; get; }
    public UiButton_ABP_Pro breakButton { private set; get; }
    public UiButton_ABP_Pro handbreakButton { private set; get; }
    public UiButton_ABP_Pro wheelieButton { private set; get; }
    public UiButton_ABP_Pro stoppieButton { private set; get; }
    public OnFallPopup onFallPopup { private set; get; }
    public ConditionMeter conditionMeter { private set; get; }
    public FuelMeter fuelMeter { private set; get; }
    public CameraAlternationButton cameraAlternation { private set; get; }
    public BikeStanceData bikeStanceData { private set; get; }
    BikeSpawner bikeSpawner;
    public ResetBikeButton resetBikeButton { private set; get; }
    public DebugConsole debugConsole { private set; get; }
    public GraphicsQualitySlider graphicsQualitySlider { private set; get; }
    public FPSLimitSlider fpsLimitSlider { private set; get; }
    Coroutine popupCoroutine;
    Transform panel;

    public event Action onBaseUIEnabled = delegate { };
    public event Action onBaseUIDisabled = delegate { };
    public LoadingScreensController loadingScreensController { private set; get; }
    private void Awake()
    { 
        SetSingletonInstance();
        gameManager = FindObjectOfType<GameManager>();
        panel = transform.GetChild(0);
        baseContextualUI = panel.GetChild(0);
        directionalController = baseContextualUI.GetChild(0).
            GetComponent<DirectionalHandlerController>();
        accelerationButton=baseContextualUI.GetChild(1).
            GetComponent<UiButton_ABP_Pro>();
        breakButton = baseContextualUI.GetChild(2).
            GetComponent<UiButton_ABP_Pro>();
        handbreakButton= baseContextualUI.GetChild(3).
            GetComponent<UiButton_ABP_Pro>();
        cameraAlternation = baseContextualUI.GetChild(7).
            GetComponent<CameraAlternationButton>();
        resetBikeButton = baseContextualUI.GetChild(8).
            GetComponent<ResetBikeButton>();
        bikeStanceData = baseContextualUI.GetChild(9).
            GetComponent<BikeStanceData>();
        conditionMeter = bikeStanceData.transform.GetChild(3).
            GetComponent<ConditionMeter>();
        fuelMeter = bikeStanceData.transform.GetChild(4).
            GetComponent<FuelMeter>();
        onFallPopup = UnityExtensionsUtilities.
            GetComponentInChildrenIncludingInactive<OnFallPopup>(transform.GetChild(0), 1);
        graphicsQualitySlider = panel.GetChild(2).GetChild(5).
            GetComponent<GraphicsQualitySlider>();
        fpsLimitSlider = panel.GetChild(2).GetChild(6).
            GetComponent<FPSLimitSlider>();
        loadingScreensController = MainMenuManager.instance.
            loadingScreensTransform.GetComponent<LoadingScreensController>();
        debugConsole = panel.GetChild(3).GetChild(1).GetComponent<DebugConsole>();
    }

    private void Start()
    {
        bikeSpawner = FindObjectOfType<BikeSpawner>();
        gameManager.bikeAdditionalPhysics.onDislodged += EnableOnFallPopup;
        gameManager.onPlayerBikeChange += UpdateSubscription;//Revisado!
    }

    void UpdateSubscription(ArcadeBikeControllerPro bikeController) =>
        bikeController.GetComponent<BikeAdditionalPhysics>().onDislodged += EnableOnFallPopup;

    public void PanelFadeIn()
    {
        var image = panel.GetComponent<Image>();
        var bikeInteractionPad = bikeSpawner.bikeInteractionPad;
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(image, 1, .75f,
            bikeInteractionPad.RaiseOnClicked));
    }

    public void PanelFadeOut()
    {
        var image = panel.GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(image, 0, .75f));

    }
    public void EnableOnFallPopup()=>
        popupCoroutine= StartCoroutine(EnableOnFallPopupAfterDelay(2));
    public void EnableResetBikeButton()=>
        resetBikeButton.gameObject.SetActive(true);
    public void DisableResetBikeButton()=>
        resetBikeButton.gameObject.SetActive(false);
    public void DisableBaseUI()
    {
        baseContextualUI.gameObject.SetActive(false);
        onBaseUIDisabled();
    }
    public void EnableBaseUI()
    {
        baseContextualUI.gameObject.SetActive(true);
        onBaseUIEnabled();
    }
    public IEnumerator EnableOnFallPopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        onFallPopup.gameObject.SetActive(true);
        DisableBaseUI();
        popupCoroutine = null;
    }
    void SetSingletonInstance()
    {
        /*if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);*/
        if (instance == null)
        {
            //First run, set the instance
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else if (instance != this)
        {
            //Instance is not the same as the one we have, destroy old one, and reset to newest one
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ReturnToMainMenu()=>
        StartCoroutine(ReturnToMainMenuAsync());

    IEnumerator ReturnToMainMenuAsync()
    {
        var operation = SceneManager.LoadSceneAsync(0);
        loadingScreensController.gameObject.SetActive(true);

        gameManager.OnReturnToMainMenu();
        
        while (!operation.isDone)
        {
            // Optionally update a loading bar or display progress
            //Debug.Log($"Loading progress: {operation.progress}");

            yield return null;
        }
       
        MainMenuManager.instance.bikeSelectionScreen.RetargetComponentsList();
        MainMenuManager.instance.mapSelectionScreen.RetargetComponentsList();
        MainMenuManager.instance.riderSelectionScreen.RetargetComponentsList();
        loadingScreensController.gameObject.SetActive(false);
        MainMenuManager.instance.gameObject.SetActive(true);
        MainMenuManager.instance.currentScreen.FadeScreenIn(true);

        gameObject.SetActive(false);
    }
}
