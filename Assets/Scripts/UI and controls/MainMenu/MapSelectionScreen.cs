using HistoricGamesUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapSelectionScreen : MainMenuScreen
{//implementar classe de base com List<T> como requisito se mais seletores forem criados!!!
    int currentMapIndex;
    //Index is 0 based because of the lists but the actual first map in the build settings starts in 1!
    public int CurrentMapIndex { get{ return currentMapIndex + 1; } }
    [SerializeField] List<GameObject> mapQuads;
    Transform mapNameTransform;
    [SerializeField]BikeSelectionScreen bikeSelectionScreen;
    [SerializeField] RiderSelectionScreen riderSelectionScreen;
    protected override void Awake()
    {
        base.Awake();
        currentMapIndex = 0;
        mapNameTransform = transform.GetChild(1).GetChild(1);
        UpdateMapData();
    }

    public void RetargetComponentsList()
    {
        var mapComponents = UnityExtensionsUtilities.
            FindObjectsOfTypeIncludingInactive<MapComponent>();
        mapQuads = new List<GameObject>();
        foreach (var map in mapComponents)
            mapQuads.Add(map.gameObject);
        currentMapIndex = 0;
    }

    public override void FadeScreenIn(bool directTransition = false)
    {
        base.FadeScreenIn();
        var quadRenderer = mapQuads[currentMapIndex].GetComponent<MeshRenderer>();
        Material quadMaterial = quadRenderer.material; // Get the material of the Quad
        if (directTransition)
        {
            throw new NotImplementedException();
        }
        else
        {
            StartCoroutine(InterpolationUtilities.
                ShaderPropertyToTargetLinearly(quadMaterial, "_Alpha", 1, .25f));
            StartCoroutine(InterpolationUtilities.
                ShaderPropertyToTargetLinearly(quadMaterial, "_Saturation", 1, .25f));
        }
        //MainMenuManager.instance.returnButton.FadeOutAndDeactivate();
        bikeSelectionScreen.FadeOutCurrentBike();
        riderSelectionScreen.FadeOutCurrentRider();
        //fade out character model
    }

    public override void FadeScreenOut(bool directTransition = false)
    {
        base.FadeScreenOut();
        var quadRenderer = mapQuads[currentMapIndex].GetComponent<MeshRenderer>();
        Material quadMaterial = quadRenderer.material; // Get the material of the Quad
        StartCoroutine(InterpolationUtilities.
            ShaderPropertyToTargetLinearly(quadMaterial, "_Alpha", .1f, .25f));
        StartCoroutine(InterpolationUtilities.
            ShaderPropertyToTargetLinearly(quadMaterial, "_Saturation", 0, .25f));
    }
    public void NextMap()
    {
        mapQuads[currentMapIndex].SetActive(false);
        int nextIndex = (currentMapIndex + 1) % mapQuads.Count;

        currentMapIndex = mapQuads.IndexOf(mapQuads[nextIndex]);
        mapQuads[currentMapIndex].SetActive(true);
        UpdateMapData();
    }

    public void PreviousMap()
    {
        mapQuads[currentMapIndex].SetActive(false);
        int previousIndex = (currentMapIndex - 1 + mapQuads.Count) % mapQuads.Count;

        currentMapIndex = mapQuads.IndexOf(mapQuads[previousIndex]);
        mapQuads[currentMapIndex].SetActive(true);
        UpdateMapData();
    }

    void UpdateMapData()
    {
        var mapName = mapQuads[currentMapIndex].GetComponent<MapComponent>().MapData.title;
        mapNameTransform.GetComponent<TextMeshProUGUI>().text = mapName;
    }
    public void FadeInCurrentMapQuadAlpha()
    {
        var quadRenderer = mapQuads[currentMapIndex].GetComponent<MeshRenderer>();
        Material quadMaterial = quadRenderer.material; // Get the material of the Quad
        if (quadMaterial.GetFloat("_Alpha") == 0)
        {
            gameObject.SetActive(true);
            StartCoroutine(InterpolationUtilities.
                ShaderPropertyToTargetLinearly(quadMaterial,
                
                "_Alpha", .1f, .25f, DisableGameObject));
        }
    }
    public void FadeOutCurrentMapQuadAlpha()
    {
        var quadRenderer = mapQuads[currentMapIndex].GetComponent<MeshRenderer>();
        Material quadMaterial = quadRenderer.material; // Get the material of the Quad
        if (quadMaterial.GetFloat("_Alpha") == .1f)
        {
            gameObject.SetActive(true);
            StartCoroutine(InterpolationUtilities.
                ShaderPropertyToTargetLinearly(quadMaterial,
                "_Alpha", 0, .25f, DisableGameObject));
        }
    }
}
