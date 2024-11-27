using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class BikeSelectionScreen : MainMenuScreen
{
    public GameObject currentBike {  get; private set; }
    public int currentBikeIndex { get; private set; }
    public void SetInitialBike() 
    { 
        currentBike = bikes[0];
        currentBikeIndex = currentBike.GetComponent<DummyBike>().BikeData.id;
    }
    [SerializeField]List<GameObject> bikes;
    Transform bikeNameTransform;
    [SerializeField]MapSelectionScreen mapSelectionScreen;
    [SerializeField]RiderSelectionScreen riderSelectionScreen;
    protected override void Awake()
    {
        base.Awake();
        //currentBike = bikes[0];
        bikeNameTransform = transform.GetChild(2).GetChild(1);
        UpdateBikeData();
    }
    public override void FadeScreenIn()
    {
        base.FadeScreenIn();
        MainMenuManager.instance.FadeCompanyLogoIn();
        MainMenuManager.instance.returnButton.FadeOutAndDeactivate();
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
        currentBikeIndex = currentBike.GetComponent<DummyBike>().BikeData.id;
        currentBike.SetActive(true);
        UpdateBikeData();
    }

    public void PreviousBike()
    {
        int currentIndex = bikes.IndexOf(currentBike);

        currentBike.SetActive(false);
        int previousIndex = (currentIndex - 1 + bikes.Count) % bikes.Count;

        currentBike = bikes[previousIndex];
        currentBikeIndex = currentBike.GetComponent<DummyBike>().BikeData.id;
        currentBike.SetActive(true);
        UpdateBikeData();
    }

    void UpdateBikeData()
    {
        var bikeName = currentBike.GetComponent<DummyBike>().BikeData.title;
        bikeNameTransform.GetComponent<TextMeshProUGUI>().text = bikeName;
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
