using HistoricGamesUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*>>>>>>>>>>>>>>>>>>>>>Summary
 *Class responsible to handle a button presence in the 3D world. 
 *The button is represented by a Quad mesh with a disabled box
 *collider component, it becomes visible and interactable when
 *the following conditions are met: The player gets closer from
 *a interaction point (in this case, an empty bike) The player
 *faces the interaction point within a certain radius threshold
 *
 */
public class BikeInteractionPad : MonoBehaviour
{
    GameManager gameManager;
    BikeSpawner bikeSpawner;
    LayerMask worldUILayer = 1 << 9;
    Camera mainCamera;
    public event Action onClicked = delegate { };
    bool isActivated, isAdEligible;
    private void Awake()
    {
        bikeSpawner = transform.parent.GetComponent<BikeSpawner>();
        gameManager = FindObjectOfType<GameManager>();
        mainCamera = Camera.main;
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
        var adIconMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        var camera = Camera.main;
        while (true)
        {
            var cameraPosition = camera.transform.position;
            var cameraForward = camera.transform.forward;

            // Calculate the direction vector to the button
            Vector3 directionToButton = (transform.position - cameraPosition).normalized;

            // Calculate the distance to the button
            float distanceToButton = Vector3.Distance(transform.position, cameraPosition);

            // Calculate the angle between the player's forward direction and the button
            float angleToButton = Vector3.Angle(cameraForward, directionToButton);

            if (gameManager.rewardedInterstitialAdController == null)
            {
                yield return null;
            }
            var isAdAvailable = gameManager.rewardedInterstitialAdController.isAvailable;
            var isAdClicked = gameManager.rewardedInterstitialAdController.isAdClicked;
            isAdEligible = (isAdAvailable && !isAdClicked);

            if (bikeSpawner.closestDistance <= 5 && angleToButton <= 15)
            {
                if (material.color.a < 1)
                {
                    StartCoroutine(InterpolationUtilities.
                        AlphaToTargetLinearly(material, 1, .5f, EnableCollider));
                }
                if (isAdEligible)
                {
                    if (adIconMaterial.color.a < 1) 
                    {
                        StartCoroutine(InterpolationUtilities.
                            AlphaToTargetLinearly(adIconMaterial, 1, .5f));
                    }
                    
                }
            }
            else
            {
                if (material.color.a > 0)
                {
                    StartCoroutine(InterpolationUtilities.
                        AlphaToTargetLinearly(material, 0, .25f, DisableCollider));
                }
                if (!isAdEligible)
                {
                    if (adIconMaterial.color.a > 0) 
                    {
                        StartCoroutine(InterpolationUtilities.
                            AlphaToTargetLinearly(adIconMaterial, 0, .25f));
                    }
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    void EnableCollider() => GetComponent<Collider>().enabled = true;
    void DisableCollider() => GetComponent<Collider>().enabled = false;
    private IEnumerator RotateButtonTowardsPlayer()
    {
        var camera = Camera.main;

        while (true)
        {
            if (bikeSpawner.closestBike == null)
            {
                yield return new WaitForSeconds(.1f);
            }
            var closestBikeTransform = bikeSpawner.closestBike.transform;

            // Calculate direction to player
            Vector3 directionToPlayer = camera.transform.position - closestBikeTransform.position;
            directionToPlayer.y = 0;

            // Normalize the direction to get proportional positioning around the bike
            Vector3 normalizedDirection = directionToPlayer.normalized;

            // Calculate target position dynamically around the bike
            Vector3 targetPosition = closestBikeTransform.position + normalizedDirection;
            targetPosition.y = closestBikeTransform.position.y + 0.5f; // Keep quad slightly above the bike

            // Update the quad's position
            transform.position = targetPosition;

            // Rotate to face the player
            Vector3 lookDirection = camera.transform.position - transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }
            yield return new WaitForSeconds(.1f);
        }
    }


    private IEnumerator CheckForInteraction()
    {
        while (true)
        {
            if (!isActivated && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
            {
                isActivated = true;
                Vector2 touchPosition;
                if (Input.touchCount > 0)
                {
                    touchPosition = Input.GetTouch(0).position;
                }
                else
                {
                    touchPosition = Input.mousePosition;
                }

                UIManager.instance.GetComponent<GraphicRaycaster>().enabled = false;
                Ray ray = mainCamera.ScreenPointToRay(touchPosition);
                RaycastHit hit;
                UIManager.instance.GetComponent<GraphicRaycaster>().enabled = true;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, worldUILayer))
                {
                    UIManager.instance.PanelFadeIn();
                    if (isAdEligible)
                    {
                        gameManager.rewardedInterstitialAdController.ShowAd();
                    }
                }
            }

            yield return new WaitUntil(() => Input.touchCount == 0 && !Input.GetMouseButton(0));
            isActivated = false;
        }
    }

}
