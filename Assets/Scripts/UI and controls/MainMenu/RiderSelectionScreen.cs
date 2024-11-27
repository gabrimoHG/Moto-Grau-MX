using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RiderSelectionScreen : MainMenuScreen
{
    public GameObject currentRider { get; private set; }
    public void SetInitialRider() => currentRider = riders[0];
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
    public override void FadeScreenIn()
    {
        base.FadeScreenIn();
        MainMenuManager.instance.FadeCompanyLogoIn();

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
    }

    public void FadeInCurrentRider()
    {
        var riderModel = currentRider.transform.GetChild(1).gameObject;
        var bikePartMaterial = riderModel.
            GetComponent<SkinnedMeshRenderer>().material;
        if (bikePartMaterial.color.a == 0)
        {
            gameObject.SetActive(true);
            StartCoroutine(InterpolationUtilities.
                FadeInObject(riderModel, .25f, DisableGameObject));
        }
    }
    public void FadeOutCurrentRider()
    {
        var riderModel = currentRider.transform.GetChild(1).gameObject;
        var bikePartMaterial = riderModel.
            GetComponent<SkinnedMeshRenderer>().material;
        if (bikePartMaterial.color.a != 0)
        {
            gameObject.SetActive(true);
            StartCoroutine(InterpolationUtilities.
                FadeOutObject(riderModel, .25f, DisableGameObject));
        }
    }
}
