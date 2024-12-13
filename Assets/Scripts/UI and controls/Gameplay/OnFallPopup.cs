using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnFallPopup : MonoBehaviour
{
    Image background, resetBikeIcon, mainMenuIcon, adIcon;
    TextMeshProUGUI titleTMPro, resetBikeTitle, mainMenuTitle;
    Button resetBikeButton, mainMenuButton;
    int disabilityCounter;
    // Start is called before the first frame update
    void Awake()
    {
        background  = GetComponent<Image>();
        titleTMPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        resetBikeButton = transform.GetChild(1).GetComponent<Button>();
        resetBikeTitle = resetBikeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        resetBikeIcon = resetBikeButton.transform.GetChild(1).GetComponent<Image>();
        adIcon = resetBikeButton.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        mainMenuButton = transform.GetChild(2).GetComponent<Button>();
        mainMenuTitle = mainMenuButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        mainMenuIcon = mainMenuButton.transform.GetChild(1).GetComponent<Image>();
        resetBikeButton.onClick.AddListener(FindObjectOfType<GameManager>().ResetBike);
        resetBikeButton.onClick.AddListener(UIManager.instance.EnableBaseUI);
        resetBikeButton.onClick.AddListener(FadeOutAndResetGraphics);
    }

    private void OnEnable()=>
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(
            background, .9f, .25f, FadeInTitle));

    void FadeInTitle() => StartCoroutine(InterpolationUtilities.
        AlphaToTargetLinearly(titleTMPro, 1, .25f, FadeOutAndMoveTitleUp));

    void FadeOutAndMoveTitleUp()
    {
        var targetPosition = new Vector2(titleTMPro.transform.localPosition.x, 256);
        var startPosition = titleTMPro.transform.localPosition;
        StartCoroutine(InterpolationUtilities.MoveToTargetLinearly(
            titleTMPro.transform, titleTMPro.transform.localPosition,
            (Vector3)targetPosition, .25f, null, true));
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(titleTMPro, .25f, .25f, FadeInButtons));
    }

    void FadeInButtons()
    {
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(resetBikeIcon, 1, .25f));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(resetBikeTitle, 1, .25f));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(mainMenuIcon, 1, .25f));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(mainMenuTitle, 1, .25f, EnableButtons));
        Debug.LogWarning("Check if there's Ad to be shown and fade in the little Ad icon here!");
        //StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(adIcon, 1, .25f,));
    }

    void EnableButtons()
    {
        resetBikeButton.enabled = true;
        mainMenuButton.enabled = true;
    }
    void FadeOutAndResetGraphics()
    {
        var targetPosition = new Vector2(titleTMPro.transform.localPosition.x, 0);
        var startPosition = titleTMPro.transform.localPosition;
        StartCoroutine(InterpolationUtilities.MoveToTargetLinearly(
            titleTMPro.transform, titleTMPro.transform.localPosition,
            (Vector3)targetPosition, .25f, DisablePopup, true));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(titleTMPro, 0, .25f, DisablePopup));
        StartCoroutine(InterpolationUtilities.
           AlphaToTargetLinearly(resetBikeIcon, 0, .25f, DisablePopup));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(resetBikeTitle, 0, .25f, DisablePopup));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(adIcon, 0, .25f, DisablePopup));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(mainMenuIcon, 0, .25f, DisablePopup));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(mainMenuTitle, 0, .25f, DisablePopup));
    }

    void DisablePopup() 
    {
        disabilityCounter++;

        if (disabilityCounter == 6)
        {
            resetBikeButton.enabled = false;
            mainMenuButton.enabled = false;
            disabilityCounter = 0;
            gameObject.SetActive(false);
        }
    }
}
