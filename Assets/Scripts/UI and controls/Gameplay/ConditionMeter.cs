using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionMeter : Gauge
{
    bool isFadeChecking;
    float normalizedCondition;
    protected override void Start()
    {
        base.Start();
        if (TMProContent != null)
            StartCoroutine(UpdateTMProInfo());
        else
        {
            bikePhysicsController.events.onConditionUpdate += UpdateImageInfo;
            gameManager.onPlayerBikeChange += UpdateSubscription;
            //UpdateImageInfo(bikePhysicsController.condition);
        }
        //UIManager.instance.onBaseUIEnabled += SetFadeCheck;
        bikePhysicsController.events.onBikeRepaired += StopFading;
        bikePhysicsController.events.onBikeBroken += SetFading;
    }

    private void OnEnable()
    {
        if (fadeLoop != null)
            StartCoroutine(FadeLoop());
    }
    private void SetFading()
    {
        fadeLoop = StartCoroutine(FadeLoop());
        bikePhysicsController.events.onBikeBroken -= SetFading;
    }

    void UpdateSubscription(ArcadeBikeControllerPro bikePhysicsController)
    {
        this.bikePhysicsController = bikePhysicsController;
        bikePhysicsController.events.onConditionUpdate += UpdateImageInfo;
        bikePhysicsController.events.onBikeRepaired += StopFading;
        bikePhysicsController.events.onBikeBroken += SetFading;
        if (bikePhysicsController.condition > 0)
            StopFading();
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

    protected override void StopFading()
    {
        if (fadeLoop != null)
        {
            StopAllCoroutines();
            fadeLoop = null;
            Color spriteColor = imageContent.color;
            spriteColor.a = 1f;
            imageContent.color = spriteColor;
            UpdateImageInfo(bikePhysicsController.condition);
            bikePhysicsController.events.onBikeBroken += SetFading;
            Debug.Log("Stop fading and color update!");
            //Debug.Log("Stop fade loop!");
        }
    }

    void UpdateImageInfo(int condition)
    {
        normalizedCondition = Mathf.Clamp01(condition / 100f);
        Color interpolatedColor = Color.Lerp(lowColor, highColor, normalizedCondition);
        imageContent.color = interpolatedColor;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        //UIManager.instance.debugConsole.
        //        AddMessage("Normalized condition " + normalizedCondition);
#endif
        //Debug.Log("Result color " + interpolatedColor);
        //Debug.Log("Normalized condition " + normalizedCondition);
    }
}
