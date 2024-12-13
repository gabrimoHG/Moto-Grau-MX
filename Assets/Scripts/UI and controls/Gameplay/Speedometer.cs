using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
using Unity.VisualScripting;
public class Speedometer : Gauge
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
        //StopAllCoroutines();
        updateInfoCoroutine = StartCoroutine(UpdateInfo()); 
    }
    IEnumerator UpdateInfo()
    {
        while (true)
        {
            TMProContent.text = bikePhysicsController.speedKmph.ToString() + " KM/H";
            yield return new WaitForSeconds(0.05f);
        }
    }
}
