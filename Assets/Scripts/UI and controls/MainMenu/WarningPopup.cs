using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningPopup : MonoBehaviour
{
    TextMeshProUGUI TMPComponent;
    Transform panelBackground, message, button;
    int graphicComponentsCount;
    // Start is called before the first frame update
    void Awake()
    {
        TMPComponent = transform.GetChild(0).
            GetChild(0).GetComponent<TextMeshProUGUI>();
        panelBackground = transform.GetChild(0);
        message = panelBackground.GetChild(0);
        button = panelBackground.GetChild(1);
        FadeIn();
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        var fadeableImage = transform.GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(fadeableImage, .75f, .25f));
        fadeableImage = panelBackground.GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(fadeableImage, 1, .25f));
        fadeableImage = button.GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(fadeableImage, 1, .25f));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(TMPComponent, 1, .25f));

    }
    public void OnClickFadeOut()
    {

        var fadeableImage = panelBackground.GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(fadeableImage, 0, .25f, OnFadeOutFinish));
        fadeableImage = button.GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(fadeableImage, 0, .25f, OnFadeOutFinish));
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(TMPComponent, 0, .25f, OnFadeOutFinish));
        fadeableImage = transform.GetComponent<Image>();
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(fadeableImage, 0, .25f, OnFadeOutFinish));
    }
    void OnFadeOutFinish()
    {
        graphicComponentsCount++;
        if (graphicComponentsCount == 4)
        {
            gameObject.SetActive(false);
            graphicComponentsCount = 0;
        }
    }
    public void SetMessage(string message)=>
        TMPComponent.text = message;  
}
