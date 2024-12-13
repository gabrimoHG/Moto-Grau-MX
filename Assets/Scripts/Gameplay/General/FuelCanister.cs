using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCanister : CollectibleItem
{
    public event Action<int> onAmountSent = delegate { };

    protected virtual void OnTriggerEnter(Collider other)
    {
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();//Revisado!
        var currentCondition = bikePhysicsController.fuelAmount;
        if (other.transform.root.gameObject == bikePhysicsController.gameObject &&
            bikePhysicsController.fuelAmount < 100)
        {
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(AudioFeedbackAndEventTrigger());
            onAmountSent(powerUpAmount);
        }
    }
}
