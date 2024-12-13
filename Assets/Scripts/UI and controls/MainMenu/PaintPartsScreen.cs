using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintPartsScreen : MainMenuScreen
{
    public int paintIndex {  get; private set; }
    FlexibleColorPicker colorPicker;
    public Color bodyColor { get; private set; }
    public Color rimsColor { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        colorPicker = transform.GetChild(2).
            GetComponent<FlexibleColorPicker>();
    }
    public void InitializeColors()
    {
        bodyColor = new Color(1, 1, 1, 1);
        rimsColor = new Color(1, 1, 1, 1);
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

    public void SetSelectededColor(Color selectedColor)
    {
        if (paintIndex == 0)
            bodyColor = selectedColor;
        else
            rimsColor = selectedColor;

    }

    public override void FadeScreenIn(bool directTransition = false)
    {
        base.FadeScreenIn();
        MainMenuManager.instance.FadeCompanyLogoOut();
        MainMenuManager.instance.returnButton.FadeInAndActivate();
        var currentBike = previousScreen.GetComponent<BikeSelectionScreen>().currentBike;
        var bodyColor = currentBike.GetComponent<BikeComponent>().GetBodyColor();
        colorPicker.color = bodyColor;
        colorPicker.onColorChange.RemoveListener(
            currentBike.GetComponent<BikeComponent>().SetRimsColor);
        colorPicker.onColorChange.AddListener(
            currentBike.GetComponent<BikeComponent>().SetBodyColor);
    }

}
