using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HistoricGamesUtils;
using System.Linq.Expressions;
using ArcadeBP_Pro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    public DirectionalHandlerController directionalController { private set; get; }
    public Transform baseContextualUI { private set; get; }
    public UiButton_ABP_Pro accelerationButton { private set; get; }
    public UiButton_ABP_Pro breakButton { private set; get; }
    public UiButton_ABP_Pro handbreakButton { private set; get; }
    public UiButton_ABP_Pro wheelieButton { private set; get; }
    public UiButton_ABP_Pro stoppieButton { private set; get; }
    public OnFallPopup onFallPopup { private set; get; }
    public ConditionMeter conditionMeter { private set; get; }
    public CameraAlternationButton cameraAlternation { private set; get; }
    Coroutine popupCoroutine;
    Transform panel;
    private void Awake()
    {
        SetSingletonInstance();
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
        conditionMeter = baseContextualUI.GetChild(5).
            GetComponent<ConditionMeter>();
        cameraAlternation = baseContextualUI.GetChild(7).
            GetComponent<CameraAlternationButton>();
        onFallPopup = UnityExtensionsUtilities.
            GetComponentInChildrenIncludingInactive<OnFallPopup>(transform.GetChild(0), 1);
    }

    private void Start()
    {
        GameManager.instance.bikeAdditionalPhysics.onDislodged += EnableOnFallPopup;
    }

    public void EnableOnFallPopup()
    {
        if (popupCoroutine == null)
            popupCoroutine= StartCoroutine(EnableOnFallPopupAfterDelay(1));
    }
    public IEnumerator EnableOnFallPopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        onFallPopup.gameObject.SetActive(true);
        popupCoroutine = null;
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
}
