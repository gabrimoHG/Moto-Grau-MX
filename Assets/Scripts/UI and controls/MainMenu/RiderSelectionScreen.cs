using ArcadeBP_Pro;
using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RiderSelectionScreen : MainMenuScreen
{//implementar classe de base com List<T> como requisito se mais seletores forem criados!!!
    public GameObject currentRider { get; private set; }
    public int currentRiderID { get; private set; }

    public void SetInitialRider()
    {
        currentRider = riders[0];
        currentRiderID = currentRider.GetComponent<RiderComponent>().RiderData.id;
    }
    [SerializeField] List<GameObject> riders;
    Transform riderNameTransform;
    [SerializeField] MapSelectionScreen mapSelectionScreen;
    [SerializeField] BikeSelectionScreen bikeSelectionScreen;
    protected override void Awake()
    {
        base.Awake();
        riderNameTransform = transform.GetChild(2).GetChild(1);
        UpdateRiderData();
    }
    public void RetargetComponentsList()
    {
        var riderComponents = UnityExtensionsUtilities.
            FindObjectsOfTypeIncludingInactive<RiderComponent>();
        riders = new List<GameObject>();
        foreach (var rider in riderComponents)
            riders.Add(rider.gameObject);
        SetInitialRider();
    }
    public override void FadeScreenIn(bool directTransition = false)
    {
        base.FadeScreenIn();
        MainMenuManager.instance.FadeCompanyLogoIn();
        //MainMenuManager.instance.returnButton.FadeOutAndDeactivate();
        mapSelectionScreen.FadeOutCurrentMapQuadAlpha();
        bikeSelectionScreen.FadeOutCurrentBike();
        //fade out character model

    }
    public void NextRider()
    {
        int currentIndex = riders.IndexOf(currentRider);

        currentRider.SetActive(false);
        int nextIndex = (currentIndex + 1) % riders.Count;

        currentRider = riders[nextIndex];
        currentRider.SetActive(true);
        UpdateRiderData();
    }

    public void PreviousRider()
    {
        int currentIndex = riders.IndexOf(currentRider);

        currentRider.SetActive(false);
        int previousIndex = (currentIndex - 1 + riders.Count) % riders.Count;

        currentRider = riders[previousIndex];
        currentRider.SetActive(true);
        UpdateRiderData();
    }

    void UpdateRiderData()
    {
        var riderName = currentRider.GetComponent<RiderComponent>().RiderData.title;
        riderNameTransform.GetComponent<TextMeshProUGUI>().text = riderName;
        currentRiderID = currentRider.GetComponent<RiderComponent>().RiderData.id;
    }

    public void FadeInCurrentRider()
    {
        var riderModels = currentRider.transform.GetComponent<RiderComponent>().models;
        foreach (var riderModel in riderModels)
        {
            var bikePartMaterial = riderModel.
                GetComponent<SkinnedMeshRenderer>().material;
            if (bikePartMaterial.color.a == 0)
            {
                gameObject.SetActive(true);
                StartCoroutine(InterpolationUtilities.
                    FadeInObject(riderModel, .25f, DisableGameObject));
            }
        }
    }
    public void FadeOutCurrentRider()
    {
        var riderModels = currentRider.transform.GetComponent<RiderComponent>().models;
        foreach (var riderModel in riderModels)
        {
            var riderPartMaterial = riderModel.
                GetComponent<SkinnedMeshRenderer>().material;
            if (riderPartMaterial.color.a != 0)
            {
                gameObject.SetActive(true);
                StartCoroutine(InterpolationUtilities.
                    FadeOutObject(riderModel, .25f, DisableGameObject));
            }
        }
    }
}
