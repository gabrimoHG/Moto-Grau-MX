using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

public class CollectibleController : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] int respawnCooldown;
    CollectibleZone playerClosestZone;
    protected List<CollectibleItem> elements;
    protected List<CollectibleZone> zones;
    protected virtual void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        SetInternalData();
    }
    void SetInternalData()
    {
        elements = new List<CollectibleItem>();
        zones = new List<CollectibleZone>();
        foreach (Transform t in transform)
            zones.Add(t.GetComponent<CollectibleZone>());
        foreach (var zone in zones)
        {
            foreach (Transform element in zone.transform)
            {
                elements.Add(element.GetComponent<CollectibleItem>());
                element.GetComponent<CollectibleItem>().onCollected += TriggerDisableTemporarily;
            }
        }
    }
    protected virtual void Start()
    {
        gameManager.onPlayerBikeChange += UpdateSubscriptionsOnBikeChange;//Revisado!
        StartCoroutine(ZonesAvailability());
        UpdateSubscriptionsOnBikeChange(gameManager.GetCurrentBikePhysicsController());//Revisado!
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ArcadeBikeControllerPro>() != null)
        {
            var bikeController = other.gameObject.GetComponent<ArcadeBikeControllerPro>();
            bikeController.isRestricted = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<ArcadeBikeControllerPro>() != null)
        {
            var bikeController = other.gameObject.GetComponent<ArcadeBikeControllerPro>();
            bikeController.isRestricted = false;
        }
    }

    void UpdateSubscriptionsOnBikeChange(ArcadeBikeControllerPro controller)
    {
        foreach (var zone in zones)
        {
            foreach (Transform element in zone.transform)
                SetSubscriptionsByItemType(element.GetComponent<CollectibleItem>());
        }
    }
    void SetSubscriptionsByItemType(CollectibleItem collectibleItem)
    {
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();//Revisado!
        switch (collectibleItem)
        {
            case FuelCanister fuelCanister:
                fuelCanister.onAmountSent += bikePhysicsController.Refuel;
                break;
            case RepairTools repairTools:
                repairTools.onAmountSent += bikePhysicsController.Repair;
                break;
            default:
                break;
        }
    }

    void TriggerDisableTemporarily(CollectibleItem collectedItem)=>
        StartCoroutine(DisableTemporarily(collectedItem));
    IEnumerator DisableTemporarily(CollectibleItem item)
    {
        item.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnCooldown);
        item.gameObject.SetActive(true);
    }

    IEnumerator ZonesAvailability()
    {
        var closestDistance = float.MaxValue;
        while (true)
        {
            var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();//Revisado!
            /*if(bikePhysicsController==null)
                yield return new WaitForSeconds(.5f);*/
            foreach (var zone in zones)
            {
                var zoneXPlayerDistance = Vector3.Distance(zone.transform.position,
                    bikePhysicsController.transform.position);
                if (zoneXPlayerDistance < closestDistance)
                {
                    playerClosestZone = zone;
                    closestDistance = zoneXPlayerDistance;
                }
            }
            foreach (var zone in zones)
            {
                if (zone == playerClosestZone)
                    playerClosestZone.gameObject.SetActive(true);
                else
                    playerClosestZone.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(.5f);
        }
    }
}