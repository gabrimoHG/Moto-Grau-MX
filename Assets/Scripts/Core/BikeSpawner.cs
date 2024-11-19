using ArcadeBP_Pro;
using Cinemachine.Utility;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class BikeSpawner : MonoBehaviour
{
    List<Transform> commonSpots, rareSpots;
    Transform commonSpotsRoot, rareSpotsRoot;
    BikesController bikesController;
    List<BikeData> spawnableBikes;
    List<Transform> spawnedBikes;
    GameObject lastBike;
    Transform lastSpot;
    public GameObject lastUsedDummyBike { get; private set; }
    public int lastCameraIndex { get; private set; }
    public BikeInteractionPad bikeInteractionPad { get; private set; }
    public GameObject closestBike {  get; private set; }
    public float closestDistance { get; private set; }
    void Awake()
    {
        commonSpotsRoot = transform.GetChild(0);
        rareSpotsRoot = transform.GetChild(1);
        commonSpots = new List<Transform>();
        rareSpots = new List<Transform>();
        bikesController = FindObjectOfType<BikesController>();
        bikeInteractionPad = transform.GetChild(2).GetComponent<BikeInteractionPad>();
        bikeInteractionPad.onClicked += EnterBike;
        foreach (Transform child in commonSpotsRoot)
            commonSpots.Add(child);
        foreach (Transform child in rareSpotsRoot)
            rareSpots.Add(child);
        closestDistance = -1f;
    }

    private void Start()
    {
        SetAvailableBikes();
        SpawnCommonBikes();
        StartCoroutine(PlayerXSpawnedBikesDistance());
    }

    void EnterBike()
    {

        //StopCoroutine(PlayerXSpawnedBikesDistance());
        StopAllCoroutines();
        bikeInteractionPad.gameObject.SetActive(false);


        var bikeController = GameManager.instance.GetCurrentBikeController();
        var bikeData = bikeController.bikeReferences.dataContainer;
        var position = bikeController.transform.position;
        var rotation = bikeController.transform.rotation;
        var bodyColor = bikeController.GetComponent<BikeComponent>().GetBodyColor();
        var rimColor = bikeController.GetComponent<BikeComponent>().GetRimsColor();
        lastCameraIndex = bikeController.bikeReferences.cameraController.currentCameraIndex;
        var condition = bikeController.condition;
        var fuelAmount = bikeController.fuelAmount;
        var isNew = bikeController.isNew;
        var isTailActive = bikeController.GetComponent<BikeComponent>().IsTailActive();
        //Debug.Log(bikeController + " isNew: " + isNew);
        Destroy(bikeController.skidmarkController.gameObject);
        Destroy(bikeController.gameObject);
        //Destroy(bikeController.bikeReferences.cameraController);
        var instance = Instantiate(bikeData.dummyPrefab, position, rotation);
        if (condition != 100 || fuelAmount != 100)
        {
            lastUsedDummyBike = instance;
            lastUsedDummyBike.GetComponent<DummyBike>().condition = condition;
            lastUsedDummyBike.GetComponent<DummyBike>().fuelAmount = fuelAmount;
            lastUsedDummyBike.GetComponent<DummyBike>().isNew = isNew;
            lastUsedDummyBike.GetComponent<BikeComponent>().SetIsTailActive(isTailActive);
        }
        /*else
            lastDummyBike = null;*/
        instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
        instance.GetComponent<BikeComponent>().SetBodyColor(rimColor);
        instance.GetComponent<DummyBike>().isShowcase = true;
        instance.GetComponent<DummyBike>().isNew = isNew;
        //Debug.Log(instance + " isNew: " + isNew);
        instance.GetComponent<Rigidbody>().isKinematic = true;
        instance.GetComponent<DummyBike>().enabled = true;
        lastBike = instance;

        lastSpot = closestBike.transform.parent;
        bikeData = closestBike.GetComponent<DummyBike>().GetBikeData();
        isNew = closestBike.GetComponent<DummyBike>().isNew;
        //Debug.Log(closestBike + " isNew: " + isNew);
        position = closestBike.transform.position;
        rotation = closestBike.transform.rotation;
        bodyColor = closestBike.GetComponent<BikeComponent>().GetBodyColor();
        rimColor = closestBike.GetComponent<BikeComponent>().GetRimsColor();
        Destroy(closestBike);
        instance = Instantiate(bikeData.mainPrefab, position, rotation);
        instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
        instance.GetComponent<BikeComponent>().SetRimsColor(rimColor);
        var cameraController = instance.GetComponent<ArcadeBikeControllerPro>().
            bikeReferences.cameraController;
        instance.GetComponent<ArcadeBikeControllerPro>().isNew = isNew;
        //Debug.Log(instance + " isNew: " + isNew);
        GameManager.instance.SetCameraController(cameraController);
        GameManager.instance.RaiseOnPlayerBikeChange(instance.
            GetComponent<ArcadeBikeControllerPro>());

        SetAvailableBikes();
        SpawnCommonBikes();
        StartCoroutine(PlayerXSpawnedBikesDistance());

    }
    void SetAvailableBikes()
    {
        spawnableBikes = new List<BikeData>(bikesController.GetBikes());
        var playerBike = GameManager.instance.GetCurrentBikeController().
            bikeReferences.dataContainer;
        for (int i = spawnableBikes.Count - 1; i >= 0; i--)
        {
            if (spawnableBikes[i].id == playerBike.id)
            {
                spawnableBikes.RemoveAt(i);
                break;
            }
        }
    }

    void SpawnCommonBikes()
    {
        var tempSpots = new List<Transform>(commonSpots);
        var bikeController = GameManager.instance.GetCurrentBikeController();
        spawnedBikes = new List<Transform>();
        if (lastSpot!=null)
            tempSpots.Remove(lastSpot);
        if (lastBike != null)
            spawnedBikes.Add(lastBike.transform);
        foreach(var bike in spawnableBikes)
        {
            var chosenSpot = GeneralUtilities.GetRandomItem(tempSpots);
            var bodyColor = GeneralUtilities.GetRandomUnityColor();
            var rimColor = GeneralUtilities.GetRandomAchromaticColor();
            if (chosenSpot.childCount > 0)
            {
                spawnedBikes.Add(chosenSpot.GetChild(0));
                continue;
            }
            if (Vector3.Distance(bikeController.transform.position, chosenSpot.position) < 10)
                continue;
            var instance = Instantiate(bike.dummyPrefab, chosenSpot.position,
                Quaternion.identity, chosenSpot);
            instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
            instance.GetComponent<BikeComponent>().SetRimsColor(rimColor);
            instance.GetComponent<DummyBike>().isShowcase = true;
            instance.GetComponent<Rigidbody>().isKinematic = true;
            spawnedBikes.Add(instance.transform);
        }
        lastSpot = null;
    }

    private IEnumerator PlayerXSpawnedBikesDistance()
    {
        var playerBike = GameManager.instance.GetCurrentBikeController().transform;
        
        while (true)
        {
            for(int i=0;i<spawnedBikes.Count;i++)
            {
                var distance = Vector3.Distance(playerBike.position,
                    spawnedBikes[i].position);
                if (i == 0)
                {
                    closestDistance = distance;
                    closestBike = spawnedBikes[i].gameObject;
                    continue;
                }
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestBike = spawnedBikes[i].gameObject;
                }
            }
            bikeInteractionPad.transform.position = new Vector3(closestBike.transform.position.x,
                closestBike.transform.position.y +1.25f, closestBike.transform.position.z-.5f);
            if(Vector3.Distance(playerBike.position, closestBike.transform.position)<10)
                bikeInteractionPad.gameObject.SetActive(true);
            else
                bikeInteractionPad.gameObject.SetActive(false);

            if (lastBike != null)
            {
                if (Vector3.Distance(playerBike.position, lastBike.transform.position) > 50)
                {
                    spawnedBikes.Remove(lastBike.transform);
                    Destroy(lastBike);
                }
            }

            yield return new WaitForSeconds(.5f);
            //yield return null;
        }
    }
}
