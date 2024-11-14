using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
public class Speedometer : Gauge
{
    protected override void Start()
    {
        base.Start();
        currentBike = GameManager.instance.GetCurrentBikeController();
        currentBike.bikeEvents.onSpeedKmphUpdate += UpdateGauge;
    }

    void UpdateGauge(int speedKmph)=>
        TMProContent.text = speedKmph.ToString() + " KM/H";
}
