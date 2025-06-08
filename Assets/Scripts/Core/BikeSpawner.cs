using ArcadeBP_Pro;
using Cinemachine.Utility;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

/*>>>>>>>>>>>>>>>>>>>>>>SUMMARY
 * Class responsible by the bike spawns and bike changes.
 * Each method has its own summary.
*/
public class BikeSpawner : MonoBehaviour
{
    GameManager gameManager;
    List<Transform> commonSpots, rareSpots;
    Transform commonSpotsRoot, rareSpotsRoot;
    BikesController bikesController;
    List<BikeData> spawnableBikes;
    List<Transform> spawnedBikes;
    //[SerializeField]List<Vector3> initialSpawnPoints;
    List<KeyValuePair<Vector3, float>> initialSpawnPoints;
     
    
    public GameObject lastBike { get; private set; }
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

    /*>>>>>>>>>>>>>>>>>>>>>>SUMMARY
     * Spawn points set, I know it's hard coded. It was built like
     * that because the game is on alpha and the map is small. A good
     * solution that comes to mind is to transfer these coordinates to
     * empty objects in the scene under a container object, adding a
     * reference to this container through a SerializedField. The coordinates
     * are composed of a position and a rotation amount in the Y axis.
     */
    public KeyValuePair<Vector3, float> SortInitialSpawnPoint()
    {
        if (initialSpawnPoints == null)
        {
            var firstPoint = new KeyValuePair<Vector3, float>(new Vector3(635.52f, -6.37f, 648.58f), 180);
            var secondPoint = new KeyValuePair<Vector3, float>(new Vector3(602.5f, -6.245f, 581.46f), 270);
            var thirdPoint = new KeyValuePair<Vector3, float>(new Vector3(467.3f, -6.18f, 508.5f), 150);
            var fourthPoint = new KeyValuePair<Vector3, float>(new Vector3(456.9f, -6.18f, 352.65f), 45);
            initialSpawnPoints = new List<KeyValuePair<Vector3, float>>();
            initialSpawnPoints.Add(firstPoint);
            initialSpawnPoints.Add(secondPoint);
            initialSpawnPoints.Add(thirdPoint);
            initialSpawnPoints.Add(fourthPoint);
        }
        return GeneralUtilities.GetRandomItem(initialSpawnPoints);
    }

    /*>>>>>>>>>>>>>>>>>>>>>>SUMMARY
     * Method responsible to handle the player's bike change,
     * it stores all pertinent data from the current bike, 
     * destroys the current bike, the new bike dummy (empty model,
     * without rider and physics) and instantiate the new bike
     * and the now previous bike dummy. The original bike data is
     * stored in case the player returns to it for data
     * consistency (gasoline, condition etc). In the end, it triggers
     * the spawn system again.
     */
    void EnterBike()
    {
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
        instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
        instance.GetComponent<BikeComponent>().SetRimsColor(rimColor);
        instance.GetComponent<DummyBike>().isShowcase = true;
        instance.GetComponent<DummyBike>().isNew = isNew;
        instance.GetComponent<Rigidbody>().isKinematic = true;
        instance.GetComponent<DummyBike>().enabled = true;
        lastBike = instance;

        bikeData = closestBike.GetComponent<DummyBike>().BikeData;
        isNew = closestBike.GetComponent<DummyBike>().isNew;
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
    /*>>>>>>>>>>>>>>>>>>>>>>SUMMARY
     * This one simply updates the spawnable bikes disregarding
     * the current one used by the player.
     */
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

    /*>>>>>>>>>>>>>>>>>>>>>>SUMMARY
     * This implementation spawns the available bikes by the
     * spawn points with random colors. It also removes the closest
     * point for the player form the list, so that the user don't
     * stumble across new bikes so easily considering his starting point.
     */
    void SpawnCommonBikes()
    {
        var tempSpots = new List<Transform>(commonSpots);
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();

        RemoveClosestSpot(bikePhysicsController.transform, tempSpots);
        spawnedBikes = new List<Transform>();

        if (lastBike != null)
        {
            spawnedBikes.Add(lastBike.transform);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            //UIManager.instance.debugConsole.AddMessage(
            //"Last player bike " + lastBike.name + " will be spawned!");
#endif
        }

        foreach (var bike in spawnableBikes)
        {
            var chosenSpot = GeneralUtilities.GetRandomItem(tempSpots);
            
            if (chosenSpot.childCount > 0)
            {
                spawnedBikes.Add(chosenSpot.GetChild(0));
                continue;
            }
            var instance = Instantiate(bike.dummyPrefab, chosenSpot.position,
                chosenSpot.localRotation, chosenSpot);
            var bodyColor = GeneralUtilities.GetRandomUnityColor();
            var rimColor = GeneralUtilities.GetRandomAchromaticColor();
            instance.GetComponent<BikeComponent>().SetBodyColor(bodyColor);
            instance.GetComponent<BikeComponent>().SetRimsColor(rimColor);
            instance.GetComponent<DummyBike>().isShowcase = true;
            instance.GetComponent<Rigidbody>().isKinematic = true;
            spawnedBikes.Add(instance.transform);

#if DEVELOPMENT_BUILD
            //UIManager.instance.debugConsole.AddMessage(
            //        "Bike " + bike.title + " spawned!");
#endif
        }
    }

    /*>>>>>>>>>>>>>>>>>>>>>>SUMMARY
     * This logic deals with the removal informed above
     */
    void RemoveClosestSpot(Transform controllerTransform,
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

    /*>>>>>>>>>>>>>>>>>>>>>>SUMMARY
     * This coroutine defines what bike is the closest
     * one from the player. It also controls if the pad
     * button (a clickable quad in the game world that
     * triggers the bike change) is active or not. Finally,
     * it destroys the player's old bike if he gets above
     * a certain distance from it.
     */
    private IEnumerator PlayerXSpawnedBikesDistance()
    {
        var playerBikeTransform = gameManager.GetCurrentBikePhysicsController().transform;

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
