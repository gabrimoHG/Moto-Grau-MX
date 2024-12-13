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
    GameManager gameManager;
    List<Transform> commonSpots, rareSpots;
    Transform commonSpotsRoot, rareSpotsRoot;
    BikesController bikesController;
    List<BikeData> spawnableBikes;
    List<Transform> spawnedBikes;
    [SerializeField]List<Vector3> initialSpawnPoints;
    public Vector3 SortInitialSpawnPoint()
    {
        return GeneralUtilities.GetRandomItem(initialSpawnPoints);
    }
    GameObject lastBike;
    Transform lastSpot;
    public GameObject lastUsedDummyBike { get; private set; }
    public int lastCameraIndex { get; private set; }
    public BikeInteractionPad bikeInteractionPad { get; private set; }
    public GameObject closestBike {  get; private set; }
    public float closestDistance { get; private set; }
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
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
        //gameManager.onInitialDataSet += InitialDataSet;
        InitialDataSet();
    }

    void InitialDataSet()
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

        var controller = gameManager.GetCurrentBikePhysicsController();
        var bikeData = controller.bikeReferences.dataContainer;

        var position = controller.transform.position;
        var rotation = controller.transform.rotation;
        var bodyColor = controller.GetComponent<BikeComponent>().GetBodyColor();
        var rimColor = controller.GetComponent<BikeComponent>().GetRimsColor();
        lastCameraIndex = controller.bikeReferences.cameraController.currentCameraIndex;
        var condition = controller.condition;
        var fuelAmount = controller.fuelAmount;
        var isNew = controller.isNew;
        var isTailActive = controller.GetComponent<BikeComponent>().IsTailActive();
        //Debug.Log(bikeController + " isNew: " + isNew);
        Destroy(controller.skidmarkController.gameObject);
        Destroy(controller.gameObject);

       


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
        instance.GetComponent<BikeComponent>().SetRimsColor(rimColor);
        instance.GetComponent<DummyBike>().isShowcase = true;
        instance.GetComponent<DummyBike>().isNew = isNew;
        //Debug.Log(instance + " isNew: " + isNew);
        instance.GetComponent<Rigidbody>().isKinematic = true;
        instance.GetComponent<DummyBike>().enabled = true;
        lastBike = instance;

        //lastSpot = closestBike.transform.parent;
        bikeData = closestBike.GetComponent<DummyBike>().BikeData;
        isNew = closestBike.GetComponent<DummyBike>().isNew;
        //Debug.Log(closestBike + " isNew: " + isNew);
        position = closestBike.transform.position;
        rotation = closestBike.transform.rotation;
        bodyColor = closestBike.GetComponent<BikeComponent>().GetBodyColor();
        rimColor = closestBike.GetComponent<BikeComponent>().GetRimsColor();
        Destroy(closestBike);
        
        instance = Instantiate(bikeData.mainPrefab, position, rotation);
        var cameraController = instance.GetComponent<ArcadeBikeControllerPro>().
            bikeReferences.cameraController;
        instance.GetComponent<ArcadeBikeControllerPro>().isNew = isNew;


        instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
        instance.GetComponent<BikeComponent>().SetRimsColor(rimColor);

        var selectedRiderID = MainMenuManager.instance.riderSelectionScreen.currentRiderID;
        var bikerController = gameManager.ridersController.GetRider(selectedRiderID);

        gameManager.SetCameraController(cameraController);
        gameManager.RaiseOnPlayerBikeChange(instance.
            GetComponent<ArcadeBikeControllerPro>());
        var parent = instance.GetComponent<ArcadeBikeControllerPro>().
            bikeReferences.bikeModel;


        var bikeInstance = Instantiate(bikerController, position, rotation, parent);

        SetAvailableBikes();
        SpawnCommonBikes();
        StartCoroutine(PlayerXSpawnedBikesDistance());

    }
    void SetAvailableBikes()
    {
        spawnableBikes = new List<BikeData>(bikesController.GetBikesData());
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController().
            bikeReferences.dataContainer;


        for (int i = spawnableBikes.Count - 1; i >= 0; i--)
        {
            if (spawnableBikes[i].id == bikePhysicsController.id)
            {
                spawnableBikes.RemoveAt(i);
                break;
            }
        }
    }

    void SpawnCommonBikes()
    {
        var tempSpots = new List<Transform>(commonSpots);
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();//Revisado!

        RemoveClosesSpot(bikePhysicsController.transform, tempSpots);
        spawnedBikes = new List<Transform>();
        /*if (lastSpot!=null)
            tempSpots.Remove(lastSpot);*/
        if (lastBike != null)
        {
            spawnedBikes.Add(lastBike.transform);
            //Debug.Log("Last bike added!");
        }

        foreach(var bike in spawnableBikes)
        {
            var chosenSpot = GeneralUtilities.GetRandomItem(tempSpots);
            
            if (chosenSpot.childCount > 0)
            {
                spawnedBikes.Add(chosenSpot.GetChild(0));
                //Debug.Log("Bike created previously added!");
                continue;
            }
            //if (Vector3.Distance(bikeController.transform.position, chosenSpot.position) < 10)
                //continue;
            var instance = Instantiate(bike.dummyPrefab, chosenSpot.position,
                Quaternion.identity, chosenSpot);
            var bodyColor = GeneralUtilities.GetRandomUnityColor();
            var rimColor = GeneralUtilities.GetRandomAchromaticColor();
            instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
            instance.GetComponent<BikeComponent>().SetRimsColor(rimColor);
            instance.GetComponent<DummyBike>().isShowcase = true;
            instance.GetComponent<Rigidbody>().isKinematic = true;
            spawnedBikes.Add(instance.transform);
            //Debug.Log("New instantiated bike added!");
        }
        //lastSpot = null;
    }
    void RemoveClosesSpot(Transform controllerTransform,
        List<Transform> spawnSpots)
    {
        for (int i = spawnSpots.Count - 1; i >= 0; i--) 
        {
            if (Vector3.Distance(controllerTransform.position, spawnSpots[i].position) < 10)
            {
                spawnSpots.Remove(spawnSpots[i]);
                break;
            }
        }
    }
    private IEnumerator PlayerXSpawnedBikesDistance()
    {
        var playerBikeTransform = gameManager.GetCurrentBikePhysicsController().transform;//Revisado!

        while (true)
        {
            for(int i=0;i<spawnedBikes.Count;i++)
            {
                var distance = Vector3.Distance(playerBikeTransform.position,
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
            /*bikeInteractionPad.transform.position = new Vector3(closestBike.transform.position.x,
                closestBike.transform.position.y +.5f, closestBike.transform.position.z);*/
            if(Vector3.Distance(playerBikeTransform.position, closestBike.transform.position)<10)
                bikeInteractionPad.gameObject.SetActive(true);
            else
                bikeInteractionPad.gameObject.SetActive(false);

            if (lastBike != null)
            {
                if (Vector3.Distance(playerBikeTransform.position, lastBike.transform.position) > 50)
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
