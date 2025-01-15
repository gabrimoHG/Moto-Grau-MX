using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearsIndicator : Gauge
{
    Coroutine updateInfoCoroutine;
    protected override void Start()
    {
        base.Start();
        updateInfoCoroutine = StartCoroutine(UpdateInfo());
    }
    void OnEnable()
    {
        if (updateInfoCoroutine == null)
            return;
        updateInfoCoroutine = StartCoroutine(UpdateInfo());
    }
    IEnumerator UpdateInfo()
    {
        while (true)
        {
            TMProContent.text = bikePhysicsController.mechanicalGear.ToString();
            yield return new WaitForSeconds(0.05f);
        }
    }

}
