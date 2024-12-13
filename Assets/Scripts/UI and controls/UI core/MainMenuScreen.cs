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
    /*private void OnDisable()
    {
        StopAllCoroutines();
    }*/
    public virtual void FadeScreenIn(bool directTransition = false)
    {
        gameObject.SetActive(true);
        
        if (imageComponents != null)
        {
            if (directTransition)
            {
                foreach (var image in imageComponents)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }
            else
            {
                foreach (var image in imageComponents)
                    StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(image, 1, .25f));
                
            }
        }
        if (textComponents != null)
        {
            if (directTransition)
            {
                foreach (var text in textComponents)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            }
            else
            {
                foreach (var text in textComponents)
                    StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(text, 1, .25f));
                
            }
        }
        //Debug.Log("Is current equal to this? " + (MainMenuManager.instance.currentScreen == this));
        if(MainMenuManager.instance.currentScreen!=this)
            MainMenuManager.instance.UpdateCurrentScreen(this);
    }
    void DebugResult()=>Debug.Log("First image alpha= " + textComponents[0].color.a);

    public virtual void FadeScreenOut(bool directTransition=false)
    {
        if (directTransition)
        {
            foreach (var image in imageComponents)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            foreach (var text in textComponents)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            gameObject.SetActive(false);
        }
        else
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
