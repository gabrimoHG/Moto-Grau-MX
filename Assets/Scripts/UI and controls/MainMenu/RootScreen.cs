using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RootScreen : MainMenuScreen
{
    protected override void Awake()
    {
        base.Awake();
        
    }
    protected override void Start()
    {
        base.Start();
        MainMenuManager.instance.UpdateCurrentScreen(this);
    }

    public override void FadeScreenIn()
    {
        base.FadeScreenIn();
        MainMenuManager.instance.returnButton.FadeOutAndDeactivate();
    }

    public override void FadeScreenOut()
    {
        base.FadeScreenOut();
    }

    public void QuitApplication() =>
        Application.Quit();
}
