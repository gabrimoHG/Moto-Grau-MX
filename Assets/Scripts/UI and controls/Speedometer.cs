using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
public class Speedometer : Gauge
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
            TMProContent.text = bikeController.speedKmph.ToString() + " KM/H";
            yield return new WaitForSeconds(0.05f);
        }
    }
}
