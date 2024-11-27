using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class FreeRoamScreen : MainMenuScreen
{
    [SerializeField]BikeSelectionScreen bikeSelectionScreen;
    [SerializeField]MapSelectionScreen mapSelectionScreen;
    [SerializeField]RiderSelectionScreen riderSelectionScreen;
    protected override void Awake()
    {
        base.Awake();
    }

    public override void FadeScreenIn()
    {
        base.FadeScreenIn();
        MainMenuManager.instance.returnButton.FadeInAndActivate();

        bikeSelectionScreen.FadeInCurrentBike();
        mapSelectionScreen.FadeInCurrentMapQuadAlpha();
        riderSelectionScreen.FadeInCurrentRider();
    }

    public override void FadeScreenOut()
    {
        base.FadeScreenOut();
    }
}
