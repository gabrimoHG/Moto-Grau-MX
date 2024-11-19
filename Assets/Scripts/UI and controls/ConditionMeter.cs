using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionMeter : Gauge
{
    protected override void Start()
    {
        base.Start();
        StartCoroutine(UpdateInfo());
    }

    IEnumerator UpdateInfo()
    {
        while (true)
        {
            TMProContent.text = bikeController.condition.ToString() + " %";
            yield return new WaitForSeconds(0.1f);
        }
    }
}
