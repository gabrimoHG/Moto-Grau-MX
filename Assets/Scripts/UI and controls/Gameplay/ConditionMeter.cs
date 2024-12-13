using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionMeter : Gauge
{
    protected override void Start()
    {
        base.Start();
        if (TMProContent != null)
            StartCoroutine(UpdateTMProInfo());
        else
        {
            bikePhysicsController.events.onConditionUpdate += UpdateImageInfo;
            gameManager.onPlayerBikeChange += UpdateSubscription;
            UpdateImageInfo(bikePhysicsController.condition);
        }
    }
    
    void UpdateSubscription(ArcadeBikeControllerPro bikePhysicsController)
    {
        this.bikePhysicsController = bikePhysicsController;
        bikePhysicsController.events.onConditionUpdate += UpdateImageInfo;
    }
    public void UpdateGauge(int condition)
    {
        if (TMProContent != null)
            TMProContent.text = condition.ToString();
        else
            UpdateImageInfo(condition);
    }
    IEnumerator UpdateTMProInfo()
    {
        while (true)
        {
            TMProContent.text = bikePhysicsController.condition.ToString() + " %";
            yield return new WaitForSeconds(0.1f);
        }
    }
    void UpdateImageInfo(int condition)
    {
        float normalizedCondition = Mathf.Clamp01(condition / 100f);
        Color interpolatedColor = Color.Lerp(lowColor, highColor, normalizedCondition);
        
        imageContent.color = interpolatedColor;
    }
}
