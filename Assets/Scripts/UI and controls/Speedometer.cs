using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeBP_Pro;
public class Speedometer : Gauge
{
    ArcadeBikeControllerPro currentBike;

    // Update is called once per frame
    void Update()
    {
        if (currentBike==null)
            currentBike = GameManager.instance.GetCurrentBike();
        TMProContent.text = currentBike.speedKmph.ToString()+" KM/H";
    }
}
