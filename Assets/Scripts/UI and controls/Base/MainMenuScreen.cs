using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField] protected MainMenuScreen previousScreen;
    public MainMenuScreen PreviousScreen() { return previousScreen; }
    protected Image[] imageComponents;
    protected TextMeshProUGUI[] textComponents;
    protected int fadeableComponentsCount;
    protected int fadeableComponentsCounter;
    protected virtual void Awake()
    {
        textComponents = GetComponentsInChildren<TextMeshProUGUI>();
        imageComponents = GetComponentsInChildren<Image>();
        fadeableComponentsCount = textComponents.Length + imageComponents.Length;
    }
    protected virtual void Start()
    {
        
    }
    public virtual void FadeScreenIn()
    {
        gameObject.SetActive(true);
        if (imageComponents != null)
        {
            foreach (var image in imageComponents)
                StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(image, 1, .25f));
        }
        if (textComponents != null)
        {
            foreach (var text in textComponents)
                StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(text, 1, .25f));
        }
        MainMenuManager.instance.UpdateCurrentScreen(this);
    }

    public virtual void FadeScreenOut()
    {
        foreach (var image in imageComponents)
        {
            StartCoroutine(InterpolationUtilities.
                AlphaToTargetLinearly(image, 0, .25f, DisableAfterFadeOut));
        }
        foreach (var text in textComponents)
        {
            StartCoroutine(InterpolationUtilities.
                AlphaToTargetLinearly(text, 0, .25f, DisableAfterFadeOut));
        }
    }
    protected virtual void DisableAfterFadeOut()
    {
        fadeableComponentsCounter++;
        if (fadeableComponentsCounter == fadeableComponentsCount)
        {
            gameObject.SetActive(false);
            fadeableComponentsCounter = 0;
        }
    }
    protected void DisableGameObject() => gameObject.SetActive(false);
}
