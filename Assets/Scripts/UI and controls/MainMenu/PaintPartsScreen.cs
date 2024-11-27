using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintPartsScreen : MainMenuScreen
{
    public int paintIndex {  get; private set; }
    FlexibleColorPicker colorPicker;


    protected override void Awake()
    {
        base.Awake();
        colorPicker = transform.GetChild(1).
            GetComponent<FlexibleColorPicker>();
    }
    /*private void OnEnable()
    {
        var currentBike = previousScreen.GetComponent<BikeSelectionScreen>().currentBike;
        var bodyColor = currentBike.GetComponent<BikeComponent>().GetBodyColor();
        colorPicker.color = bodyColor;
        colorPicker.onColorChange.RemoveListener(
            currentBike.GetComponent<BikeComponent>().SetRimsColor);
        colorPicker.onColorChange.AddListener(
            currentBike.GetComponent<BikeComponent>().SetBodyColor);
        
    }*/
    public void SetData(int paintIndex)
    {
        this.paintIndex = paintIndex;
        var currentBike = previousScreen.GetComponent<BikeSelectionScreen>().currentBike;
        var bikeComponent = currentBike.GetComponent<BikeComponent>();
        var color = new Color();
        //colorPicker.onColorChange.RemoveAllListeners();
        if (paintIndex == 0)
        {
            colorPicker.onColorChange.RemoveListener(bikeComponent.SetRimsColor);
            color = bikeComponent.GetBodyColor();
            colorPicker.onColorChange.AddListener(bikeComponent.SetBodyColor);
        }
        else
        {
            colorPicker.onColorChange.RemoveListener(bikeComponent.SetBodyColor);
            color = bikeComponent.GetRimsColor();
            colorPicker.onColorChange.AddListener(bikeComponent.SetRimsColor);
        }
        colorPicker.color = color;

    }

    public override void FadeScreenIn()
    {
        base.FadeScreenIn();
        MainMenuManager.instance.FadeCompanyLogoOut();
        var currentBike = previousScreen.GetComponent<BikeSelectionScreen>().currentBike;
        var bodyColor = currentBike.GetComponent<BikeComponent>().GetBodyColor();
        colorPicker.color = bodyColor;
        colorPicker.onColorChange.RemoveListener(
            currentBike.GetComponent<BikeComponent>().SetRimsColor);
        colorPicker.onColorChange.AddListener(
            currentBike.GetComponent<BikeComponent>().SetBodyColor);
    }

}
