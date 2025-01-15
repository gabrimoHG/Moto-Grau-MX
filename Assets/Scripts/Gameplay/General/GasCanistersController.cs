using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCanistersController : CollectibleController
{
    public void Unsubscription(ArcadeBikeControllerPro bikePhysicsController)
    {
        foreach (var collectible in collectibles)
        {
            var fuelCanister = (FuelCanister)collectible;
            fuelCanister.onAmountSent -= bikePhysicsController.Refuel;
            //Debug.Log("Fuel canister unsubscribed!");
        }
    }
}
