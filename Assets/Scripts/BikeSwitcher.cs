using UnityEngine;
using UnityEngine.UI;
using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;

public class BikeSwitcher : MonoBehaviour
{
    List<ArcadeBikeControllerPro> bikes;
    private int currentBikeIndex = 0;

    /*private void Awake()
    {
#if UNITY_EDITOR
        bikes = new List<ArcadeBikeControllerPro>();
        var bikesController = FindObjectOfType<BikesController>();
        for(int i = 0; i < bikesController.GetBikes().Count; i++)
            bikes.Add(bikesController.GetDevBike(i));
#else
        List<BikePhysicsController> bikes;
        bikes = new List<BikePhysicsController>();
        var bikesController = FindObjectOfType<BikesController>();
        for(int i = 0; i < bikesController.GetBikes().Count; i++)
            bikes.Add(bikesController.GetBike(i));
#endif
    }*/

    public void SwitchToNextBike()
    {
        DisableBike(currentBikeIndex);
        currentBikeIndex = (currentBikeIndex + 1) % bikes.Count;
        EnableBike(currentBikeIndex);
    }

    public void SwitchToPreviousBike()
    {
        DisableBike(currentBikeIndex);
        currentBikeIndex = (currentBikeIndex - 1 + bikes.Count) % bikes.Count;
        EnableBike(currentBikeIndex);
    }

    public ArcadeBikeControllerPro GetCurrentBike()
    {
        if (bikes.Count > 0 && currentBikeIndex >= 0 &&
        currentBikeIndex < bikes.Count)
        {
            return bikes[currentBikeIndex];
        }
        return null;
    }

    private void EnableBike(int index)
    {
        if (index >= 0 && index < bikes.Count)
            bikes[index].gameObject.SetActive(true);
    }

    private void DisableBike(int index)
    {
        if (index >= 0 && index < bikes.Count)
            bikes[index].gameObject.SetActive(false);
    }
}
