using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikesController : MonoBehaviour
{
    [SerializeField]List<BikeData> bikes;
    public List<BikeData> GetBikes() { return bikes; }
    public ArcadeBikeControllerPro GetBike(int id)
    {
        foreach(var bike in bikes)
        {
            if (bike.id == id)
                return bike.mainPrefab.GetComponent<ArcadeBikeControllerPro>();
        }
        return null;
    }

}
