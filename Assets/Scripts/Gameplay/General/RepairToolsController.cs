using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairToolsController : CollectibleController
{
    public void Unsubscription(ArcadeBikeControllerPro bikePhysicsController)
    {
        foreach (var collectible in collectibles)
        {
            var repairTools = (RepairTools)collectible;
            repairTools.onAmountSent -= bikePhysicsController.Repair;
            //Debug.Log("Repair tools unsubscribed!");
        }
    }
}
