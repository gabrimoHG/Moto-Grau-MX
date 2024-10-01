using UnityEngine;
using UnityEngine.UI;
using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;

public class BikeSwitcher : MonoBehaviour
{
    [SerializeField]List<ArcadeBikeControllerPro> bikes;

    private int currentBikeIndex = 0;


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
