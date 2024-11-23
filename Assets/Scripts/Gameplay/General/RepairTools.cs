using ArcadeBP_Pro;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class RepairTools : CollectibleItem
{
    public event Action<int> onAmountSent = delegate { };

    protected virtual void OnTriggerEnter(Collider other)
    {
        var bikeController = GameManager.instance.GetCurrentBikeController();
        var currentCondition = bikeController.condition; ;
        if (other.transform.root.gameObject == bikeController.gameObject &&
            bikeController.condition<100)
        {
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(AudioFeedbackAndEventTrigger());
            onAmountSent(powerUpAmount);
        }
    }
}

