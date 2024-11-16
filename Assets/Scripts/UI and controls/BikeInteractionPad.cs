using HistoricGamesUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BikeInteractionPad : MonoBehaviour
{
    BikeSpawner bikeSpawner;
    LayerMask worldUILayer = 1 << 9;
    Image directionalHandlerImage;
    Camera mainCamera;
    GraphicRaycaster raycaster;
    public event Action onClicked = delegate { };
    private void Awake()
    {
        bikeSpawner = transform.parent.GetComponent<BikeSpawner>();
        mainCamera = Camera.main;
    }
    private void Start()
    {
        directionalHandlerImage = UIManager.instance.directionalController.GetComponent<Image>();
        raycaster = UIManager.instance.GetComponent<GraphicRaycaster>();
    }
    private void OnEnable()
    {
        StartCoroutine(RotateButtonTowardsPlayer());
        StartCoroutine(EnableButtonVisibility());
        StartCoroutine(CheckForInteraction());
    }

    void OnDisable() => StopAllCoroutines();

    public void RaiseOnClicked() 
    { 
        onClicked();
        UIManager.instance.PanelFadeOut();
    }
    IEnumerator EnableButtonVisibility()
    {
        var material = GetComponent<MeshRenderer>().material;
        while (true)
        {
            if (bikeSpawner.closestDistance <= 5)
            {
                if (material.color.a < 1)
                    StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(material, 1, .5f));
            }
            else
            {
                if (material.color.a > 0)
                    StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(material, 0, .5f));
            }

            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator RotateButtonTowardsPlayer()
    {
        var camera = Camera.main;
        while (true)
        {
            Vector3 directionToPlayer = camera.transform.position - transform.position;
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = targetRotation;
            }

            yield return new WaitForSeconds(.1f);
        }
    }


    private IEnumerator CheckForInteraction()
    {
        while (true)
        {
            if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
            {
                Vector2 touchPosition;

                if (Input.touchCount > 0)
                    touchPosition = Input.GetTouch(0).position;
                else
                    touchPosition = Input.mousePosition;

                UIManager.instance.GetComponent<GraphicRaycaster>().enabled = false;
                Ray ray = mainCamera.ScreenPointToRay(touchPosition);
                RaycastHit hit;
                UIManager.instance.GetComponent<GraphicRaycaster>().enabled = true;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, worldUILayer))
                    UIManager.instance.PanelFadeIn();
            }
                
            yield return new WaitForSeconds(.01f);
            //yield return null;
        }
    }



}
