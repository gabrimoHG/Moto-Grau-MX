using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionMeter : Gauge
{
    protected override void Start()
    {
        base.Start();
        currentBike = GameManager.instance.GetCurrentBikeController();
        currentBike.bikeEvents.onConditionUpdate += UpdateGauge;
    }

    public void UpdateGauge(int bikeCondition) =>
        TMProContent.text = bikeCondition.ToString() + " %";
}
