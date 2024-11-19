using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : Gauge
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdateInfo());
    }

    public void UpdateGauge(int bikeFuel) =>
        TMProContent.text = bikeFuel.ToString();

    IEnumerator UpdateInfo()
    {
        while (true)
        {
            TMProContent.text = bikeController.fuelAmount.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
