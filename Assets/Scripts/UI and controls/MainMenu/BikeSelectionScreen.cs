using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class BikeSelectionScreen : MainMenuScreen
{//implementar classe de base com List<T> como requisito se mais seletores forem criados!!!
    public GameObject currentBike {  get; private set; }
    public static int currentBikeID { get; private set; }
    public void SetInitialBike() 
    {
        //currentBike = bikes[0];
        //currentBikeID = currentBike.GetComponent<DummyBike>().BikeData.id;
        foreach (var bike in bikes)
        {
            if (currentBikeID == bike.GetComponent<DummyBike>().BikeData.id)
            {
                currentBike = bike;
                currentBike.SetActive(true);
            }
            else
                bike.SetActive(false);
        }
    }
    [SerializeField]List<GameObject> bikes;
    Transform bikeNameTransform;
    [SerializeField]MapSelectionScreen mapSelectionScreen;
    [SerializeField]RiderSelectionScreen riderSelectionScreen;
    protected override void Awake()
    {
        base.Awake();
        /*foreach(var bike in bikes)
            DontDestroyOnLoad(bike);*/
        //currentBike = bikes[0];
        bikeNameTransform = transform.GetChild(2).GetChild(1);
        UpdateBikeData();
    }
    public void RetargetComponentsList()
    {
        var bikeComponents = UnityExtensionsUtilities.
            FindObjectsOfTypeIncludingInactive<BikeComponent>();
        bikes = new List<GameObject>();
        //Debug.Log("ID received in Main Menu "+currentBikeID);
        foreach (var bikeComponent in bikeComponents)
        {
            bikes.Add(bikeComponent.gameObject);
            var dummyBike = bikeComponent.GetComponent<DummyBike>();
            if (currentBikeID == dummyBike.BikeData.id)
                currentBike = bikeComponent.gameObject;
        }
        //SetInitialBike();

    }

    public override void FadeScreenIn(bool directTransition = false)
    {
        base.FadeScreenIn();
        MainMenuManager.instance.FadeCompanyLogoIn();
        //MainMenuManager.instance.returnButton.FadeOutAndDeactivate();
        mapSelectionScreen.FadeOutCurrentMapQuadAlpha();
        riderSelectionScreen.FadeOutCurrentRider();
        //fade out character model

    }
    public void NextBike()
    {
        int currentIndex = bikes.IndexOf(currentBike);

        currentBike.SetActive(false);
        int nextIndex = (currentIndex + 1) % bikes.Count;

        currentBike = bikes[nextIndex];
        currentBike.SetActive(true);
        UpdateBikeData();
    }

    public void PreviousBike()
    {
        int currentIndex = bikes.IndexOf(currentBike);

        currentBike.SetActive(false);
        int previousIndex = (currentIndex - 1 + bikes.Count) % bikes.Count;

        currentBike = bikes[previousIndex];
        currentBike.SetActive(true);
        UpdateBikeData();
    }

    void UpdateBikeData()
    {
        var bikeName = currentBike.GetComponent<DummyBike>().BikeData.title;
        bikeNameTransform.GetComponent<TextMeshProUGUI>().text = bikeName;
        currentBikeID = currentBike.GetComponent<DummyBike>().BikeData.id;
    }

    public void FadeInCurrentBike()
    {
        var bikeParts = currentBike.GetComponent<BikeComponent>().models;
        var bikePartMaterial = bikeParts[0].GetComponent<MeshRenderer>().material;
        if (bikePartMaterial.color.a == 0)
        {
            gameObject.SetActive(true);
            StartCoroutine(InterpolationUtilities.
                FadeInObject(bikeParts, .25f, DisableGameObject));
        }
    }
    public void FadeOutCurrentBike()
    {
        var bikeParts = currentBike.GetComponent<BikeComponent>().models;
        var bikePartMaterial = bikeParts[0].GetComponent<MeshRenderer>().material;
        if (bikePartMaterial.color.a != 0)
        {
            gameObject.SetActive(true);
            StartCoroutine(InterpolationUtilities.
                FadeOutObject(bikeParts, .25f, DisableGameObject));
        }
    }
}
