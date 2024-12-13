using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : Gauge
{
    protected override void Start()
    {
        base.Start();
        if (TMProContent != null)
            StartCoroutine(UpdateTMProInfo());
        else
        {
            bikePhysicsController.events.onFuelUpdate += UpdateImageInfo;
            gameManager.onPlayerBikeChange += UpdateSubscription;
            UpdateImageInfo(bikePhysicsController.fuelAmount);
        }
    }
    void UpdateSubscription(ArcadeBikeControllerPro bikePhysicsController)
    {
        this.bikePhysicsController = bikePhysicsController;
        bikePhysicsController.events.onFuelUpdate += UpdateImageInfo;
    }
    public void UpdateGauge(int bikeFuel)
    {
        if (TMProContent != null)
            TMProContent.text = bikeFuel.ToString();
        else
            UpdateImageInfo(bikeFuel);
    }

    IEnumerator UpdateTMProInfo()
    {
        while (true)
        {
            TMProContent.text = bikePhysicsController.fuelAmount.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void UpdateImageInfo(int currentFuel)
    {
        float normalizedFuel = Mathf.Clamp01(currentFuel / 100f);
        Color interpolatedColor = Color.Lerp(lowColor, highColor, normalizedFuel);

        imageContent.color = interpolatedColor;
    }
}
