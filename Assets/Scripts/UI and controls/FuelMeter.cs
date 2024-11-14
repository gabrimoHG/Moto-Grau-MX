using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : Gauge
{
    protected override void Start()
    {
        base.Start();
        currentBike = GameManager.instance.GetCurrentBikeController();
        currentBike.bikeEvents.onFuelUpdate += UpdateGauge;
    }

    public void UpdateGauge(int bikeFuel) =>
        TMProContent.text = bikeFuel.ToString();
}
