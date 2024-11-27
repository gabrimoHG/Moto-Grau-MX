using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;
public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager instance { get; private set; }
    public MainMenuScreen currentScreen { get; private set; }
    public void UpdateCurrentScreen(MainMenuScreen currentScreen)
    {
        if(this.currentScreen!=null)
            this.currentScreen.FadeScreenOut();
        this.currentScreen = currentScreen;
    }
    public Transform logos { get; private set; }
    public Transform rootScreen { get; private set; }
    public Transform loadingScreensContainer { get; private set; }
    public Transform freeRoamContainer { get; private set; }
    public BikeSelectionScreen bikeSelectionScreen { get; private set; }
    public RiderSelectionScreen riderSelectionScreen { get; private set; }
    public MapSelectionScreen mapSelectionScreen { get; private set; }
    public PaintPartsScreen paintPartsScreen { get; private set; }
    public FreeRoamScreen freeRoamScreen { get; private set; }
    public ReturnButton returnButton { get; private set; }
    
    public WarningPopup warningPopup { get; private set; }
    private void Awake()
    {
        SetSingletonInstance();
        logos = transform.GetChild(0);
        rootScreen = transform.GetChild(1);
        freeRoamContainer = transform.GetChild(2);
        var lastChileID = transform.childCount - 1;
        warningPopup = transform.GetChild(lastChileID).GetComponent<WarningPopup>();
        lastChileID = transform.childCount - 2;
        returnButton = transform.GetChild(lastChileID).GetComponent<ReturnButton>();
        loadingScreensContainer = transform.GetChild(4);
        freeRoamScreen = freeRoamContainer.GetChild(0).GetComponent<FreeRoamScreen>();
        bikeSelectionScreen = freeRoamContainer.GetChild(1).GetComponent<BikeSelectionScreen>();
        paintPartsScreen = freeRoamContainer.GetChild(2).GetComponent<PaintPartsScreen>();
        mapSelectionScreen = freeRoamContainer.GetChild(3).GetComponent<MapSelectionScreen>();
        riderSelectionScreen = freeRoamContainer.GetChild(4).GetComponent<RiderSelectionScreen>();
        loadingScreensContainer = transform.GetChild(4);
        bikeSelectionScreen.SetInitialBike();
        riderSelectionScreen.SetInitialRider();
    }
    void Start()
    {

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
        loadingScreensContainer.gameObject.SetActive(true);
        while (!operation.isDone)
        {
            // Optionally update a loading bar or display progress
            //Debug.Log($"Loading progress: {operation.progress}");
            
            yield return null;
        }
        loadingScreensContainer.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
