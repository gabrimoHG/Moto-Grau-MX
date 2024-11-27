using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnButton : MainMenuButton
{
    Image image;

    protected override void Awake()
    {
        base.Awake();
        image = GetComponent<Image>();
    }
    public override void OnClick()
    {
        var currentScreen = MainMenuManager.instance.currentScreen;
        if (currentScreen == null)
            return;
        currentScreen.PreviousScreen().FadeScreenIn();
}
    public void FadeInAndActivate()
    {
        if (gameObject.activeSelf)
            return;
        gameObject.SetActive(true);
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(image, 1, .25f));
    }
    public void FadeOutAndDeactivate()
    {
        StartCoroutine(InterpolationUtilities.
            AlphaToTargetLinearly(image, 0, .25f, DisableGameobject));
    }
    void DisableGameobject()=>gameObject.SetActive(false);
}
