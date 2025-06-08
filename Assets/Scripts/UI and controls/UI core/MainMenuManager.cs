using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;
using ArcadeBP_Pro;
using GoogleMobileAds.Api;
public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager instance { get; private set; }
    public MainMenuScreen currentScreen { get; private set; }
    public void UpdateCurrentScreen(MainMenuScreen currentScreen, 
        bool directTransition=false)
    {
        if (this.currentScreen != null)
            this.currentScreen.FadeScreenOut(directTransition);
        this.currentScreen = currentScreen;

    }
    public Transform logos { get; private set; }
    public Transform rootScreenTransform { get; private set; }
    public Transform loadingScreensTransform { get; private set; }
    public Transform freeRoamTransform { get; private set; }
    public RootScreen rootScreen { get; private set; }
    public BikeSelectionScreen bikeSelectionScreen { get; private set; }
    public RiderSelectionScreen riderSelectionScreen { get; private set; }
    public MapSelectionScreen mapSelectionScreen { get; private set; }
    public PaintPartsScreen paintPartsScreen { get; private set; }
    public FreeRoamScreen freeRoamScreen { get; private set; }
    public ReturnButton returnButton { get; private set; }
    public WarningPopup warningPopup { get; private set; }
    public bool isKeyCollected { get; private set; }
    private void Awake()
    {
        SetSingletonInstance();
        //CleanUpLastSessionVariables();
        logos = transform.GetChild(0);
        rootScreenTransform = transform.GetChild(1);
        rootScreen = rootScreenTransform.GetComponent<RootScreen>();
        freeRoamTransform = transform.GetChild(2);
        var lastChildID = transform.childCount - 1;
        warningPopup = transform.GetChild(lastChildID).GetComponent<WarningPopup>();
        returnButton = transform.GetChild(4).GetComponent<ReturnButton>();
        loadingScreensTransform = transform.GetChild(5);
        freeRoamScreen = freeRoamTransform.GetChild(0).GetComponent<FreeRoamScreen>();
        bikeSelectionScreen = freeRoamTransform.GetChild(1).GetComponent<BikeSelectionScreen>();
        paintPartsScreen = freeRoamTransform.GetChild(2).GetComponent<PaintPartsScreen>();
        paintPartsScreen.InitializeColors();
        mapSelectionScreen = freeRoamTransform.GetChild(3).GetComponent<MapSelectionScreen>();
        riderSelectionScreen = freeRoamTransform.GetChild(4).GetComponent<RiderSelectionScreen>();
        bikeSelectionScreen.SetInitialBike();
        riderSelectionScreen.SetInitialRider();
    }
    private void Start()
    {
        AdmobInitialization();
        SetSessionData();
    }

    void AdmobInitialization()
    {
        RequestConfiguration requestConfiguration = new RequestConfiguration
        {
            TestDeviceIds = new List<string> {
                "f4808a71-c948-4746-9665-dff976aedd3e",
                "e6f2cf80-4aa6-461f-b61c-cd01d0ee9de3"}
        };

        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(initStatus => { });
    }

    void SetSessionData()
    {
        var currentDate = System.DateTime.Now.ToString("yyyyMMdd");
        PlayerPrefs.SetString("LastPlayedDate", currentDate);
        var isKeyCollectedInt = PlayerPrefs.GetInt("IsNextBikeKeyCollected");
        if (isKeyCollectedInt == 0)
            isKeyCollected = false;
        else
            isKeyCollected = true;
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

    private void OnEnable()
    {
        if(currentScreen!=null)
        {
            if(currentScreen!=rootScreen)
                UpdateCurrentScreen(rootScreen);
        }
    }
    /*private void OnDisable()
    {
        StopAllCoroutines();
    }*/
    //public void ReturnToRootScreen() => UpdateCurrentScreen(rootScreen);

    public void FadeCompanyLogoOut()
    {
        var companyLogo = logos.GetChild(1).GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(companyLogo, 0, .25f));
    }
    public void FadeCompanyLogoIn()
    {
        var companyLogo = logos.GetChild(1).GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(companyLogo, .5f, .25f));
    }
    public void FadeMainScreenOut()
    {

    }

    public void FadeFreeRoamScreenOut()
    {

    }
    public void LoadFreeFreeRoamScene()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        var currentMapIndex = mapSelectionScreen.CurrentMapIndex;
        var operation = SceneManager.LoadSceneAsync(currentMapIndex);
        loadingScreensTransform.gameObject.SetActive(true);
        UpdateCurrentScreen(rootScreen, true);

        while (!operation.isDone)
        {
            // Optionally update a loading bar or display progress
            //Debug.Log($"Loading progress: {operation.progress}");
            
            yield return null;
        }
        loadingScreensTransform.gameObject.SetActive(false);
        UIManager.instance.gameObject.SetActive(true);
        gameObject.SetActive(false);
        //Debug.Break();
    }
}
