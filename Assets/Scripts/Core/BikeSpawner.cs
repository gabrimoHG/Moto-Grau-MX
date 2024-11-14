using ArcadeBP_Pro;
using Cinemachine.Utility;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BikeSpawner : MonoBehaviour
{
    Dictionary<Transform, Vector3> commonSpots, rareSpots;
    Transform commonSpotsRoot, rareSpotsRoot;
    BikesController bikesController;
    List<BikeData> spawnableBikes;
    List<Transform> spawnedBikes;
    Transform bikeInteractionUI;
    public GameObject closestBike {  get; private set; }
    public float closestDistance { get; private set; }
    void Awake()
    {
        commonSpotsRoot = transform.GetChild(0);
        rareSpotsRoot = transform.GetChild(1);
        commonSpots = new Dictionary<Transform, Vector3>();
        rareSpots = new Dictionary<Transform, Vector3>();
        spawnedBikes = new List<Transform>();
        bikesController = FindObjectOfType<BikesController>();
        bikeInteractionUI = transform.GetChild(2);
        foreach (Transform child in commonSpotsRoot)
            commonSpots.Add(child, child.position);
        foreach (Transform child in rareSpotsRoot)
            rareSpots.Add(child, child.position);
        closestDistance = -1f;
    }

    private void Start()
    {
        SetAvailableBikes();
        SpawnBikes();
        StartCoroutine(PlayerXSpawnedBikesDistance());
    }

    public void EnterBike()
    {

        StopCoroutine(PlayerXSpawnedBikesDistance());
        bikeInteractionUI.gameObject.SetActive(false);


        var bikeController = GameManager.instance.GetCurrentBikeController();
        var bikeData = bikeController.bikeReferences.dataContainer;
        var position = bikeController.transform.position;
        var rotation = bikeController.transform.rotation;
        var bodyColor = bikeController.GetComponent<BikeBody>().GetBodyColor();
        var rimColor = bikeController.GetComponent<BikeRim>().GetRimColor();
        Destroy(bikeController.gameObject);
        var instance = Instantiate(bikeData.dummyPrefab, position, rotation);
        instance.GetComponent<BikeBody>().SetPartColor(bodyColor);
        instance.GetComponent<BikeRim>().SetPartColor(rimColor);

        bikeData = closestBike.GetComponent<DummyBike>().GetBikeData();
        position = closestBike.transform.position;
        rotation = closestBike.transform.rotation;
        bodyColor = closestBike.GetComponent<BikeBody>().GetBodyColor();
        rimColor = closestBike.GetComponent<BikeRim>().GetRimColor();
        Destroy(closestBike);
        instance = Instantiate(bikeData.mainPrefab, position, rotation);
        instance.GetComponent<BikeBody>().SetPartColor(bodyColor);
        instance.GetComponent<BikeRim>().SetPartColor(rimColor);
        GameManager.instance.RaiseOnPlayerBikeChange(instance.
            GetComponent<ArcadeBikeControllerPro>());

        
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

    void SpawnBikes()
    {
        var tempSpots = new Dictionary<Transform, Vector3>(commonSpots);
        foreach(var bike in spawnableBikes)
        {
            var chosenSpot = GeneralUtilities.GetRandomElement(tempSpots);
            var bodyColor = GeneralUtilities.GetRandomUnityColor();
            var rimColor = GeneralUtilities.GetRandomAchromaticColor();

            var instance = Instantiate(bike.dummyPrefab, chosenSpot.Value,
                Quaternion.identity, chosenSpot.Key);
            instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
            instance.GetComponent<BikeComponent>().SetRimColor(rimColor);
            instance.GetComponent<DummyBike>().isShowcase = true;
            instance.GetComponent<Rigidbody>().isKinematic = true;
            spawnedBikes.Add(instance.transform);
        }
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
            bikeInteractionUI.position = new Vector3(closestBike.transform.position.x,
                closestBike.transform.position.y +1.25f, closestBike.transform.position.z-.5f);
            //Debug.Log("Distance -- " + Vector3.Distance(playerBike.position, closestBike.transform.position));
            if(Vector3.Distance(playerBike.position, closestBike.transform.position)<10)
                bikeInteractionUI.gameObject.SetActive(true);
            else
                bikeInteractionUI.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);
        }
    }
}
