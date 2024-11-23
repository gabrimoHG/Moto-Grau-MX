using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCanister : CollectibleItem
{
    public event Action<int> onAmountSent = delegate { };

    protected virtual void OnTriggerEnter(Collider other)
    {
        var bikeController = GameManager.instance.GetCurrentBikeController();
        var currentCondition = bikeController.fuelAmount;
        if (other.transform.root.gameObject == bikeController.gameObject &&
            bikeController.fuelAmount < 100)
        {
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(AudioFeedbackAndEventTrigger());
            onAmountSent(powerUpAmount);
        }
    }
}
