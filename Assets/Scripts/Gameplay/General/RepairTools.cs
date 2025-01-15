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
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();//Revisado!
        var currentCondition = bikePhysicsController.condition;
        if (other.transform.root.gameObject == bikePhysicsController.gameObject &&
            bikePhysicsController.condition<100)
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(AudioFeedbackAndEventTrigger());
            onAmountSent(powerUpAmount);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            //UIManager.instance.debugConsole.
            //    AddMessage("Repair triggered!");
#endif
            //Debug.Log("Power up event triggered!");
        }
    }
}

