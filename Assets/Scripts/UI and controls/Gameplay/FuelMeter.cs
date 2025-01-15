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
            //UpdateImageInfo(bikePhysicsController.fuelAmount);
        }
        bikePhysicsController.events.onBikeRefueled += StopFading;
        bikePhysicsController.events.onBikeOutOfFuel += SetFading;
    }
    private void SetFading() =>
        fadeLoop = StartCoroutine(FadeLoop());
    void UpdateSubscription(ArcadeBikeControllerPro bikePhysicsController)
    {
        this.bikePhysicsController = bikePhysicsController;
        bikePhysicsController.events.onFuelUpdate += UpdateImageInfo;
        if (bikePhysicsController.fuelAmount > 0)
            StopFading();
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
    protected override void StopFading()
    {
        if (fadeLoop != null)
        {
            StopCoroutine(fadeLoop);
            StopCoroutine(Fade(1 ,1));
            fadeLoop = null;
            Color spriteColor = imageContent.color;
            spriteColor.a = 1f;
            imageContent.color = spriteColor;
            UpdateImageInfo(bikePhysicsController.fuelAmount);
            //Debug.Log("Stop fade loop!");
        }
    }
    void UpdateImageInfo(int currentFuel)
    {
        float normalizedFuel = Mathf.Clamp01(currentFuel / 100f);
        Color interpolatedColor = Color.Lerp(lowColor, highColor, normalizedFuel);
        imageContent.color = interpolatedColor;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        //UIManager.instance.debugConsole.
        //        AddMessage("Normalized fuel " + normalizedFuel);
#endif
        //Debug.Log("Normalized fuel " + normalizedFuel);
    }
}
