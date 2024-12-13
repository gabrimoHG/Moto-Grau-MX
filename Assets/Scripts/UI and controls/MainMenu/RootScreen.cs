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

    public override void FadeScreenIn(bool directTransition = false)
    {
        base.FadeScreenIn(directTransition);
        MainMenuManager.instance.returnButton.FadeOutAndDeactivate(directTransition);
    }

    public override void FadeScreenOut(bool directTransition = false)
    {
        base.FadeScreenOut(directTransition);
    }

    public void QuitApplication() =>
        Application.Quit();
}
