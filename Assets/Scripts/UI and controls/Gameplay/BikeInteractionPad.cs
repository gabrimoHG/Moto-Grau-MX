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
    bool isActivated;
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
        var camera = Camera.main;
        while (true)
        {
            /*if (GameManager.instance == null)
                yield return new WaitForSeconds(.1f);*/
            var cameraPosition = camera.transform.position;
            var cameraForward = camera.transform.forward;

            // Calculate the direction vector to the button
            Vector3 directionToButton = (transform.position - cameraPosition).normalized;

            // Calculate the distance to the button
            float distanceToButton = Vector3.Distance(transform.position, cameraPosition);

            // Calculate the angle between the player's forward direction and the button
            float angleToButton = Vector3.Angle(cameraForward, directionToButton);

            if (bikeSpawner.closestDistance <= 5 && angleToButton <= 15)
            {
                if (material.color.a < 1)
                    StartCoroutine(InterpolationUtilities.
                        AlphaToTargetLinearly(material, 1, .5f, EnableCollider));
            }
            else
            {
                if (material.color.a > 0)
                    StartCoroutine(InterpolationUtilities.
                        AlphaToTargetLinearly(material, 0, .25f, DisableCollider));
            }
            //yield return null;
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
                yield return new WaitForSeconds(.1f);
            var closestBikeTransform = bikeSpawner.closestBike.transform;

            // Calculate direction to player
            Vector3 directionToPlayer = camera.transform.position - closestBikeTransform.position;
            directionToPlayer.y = 0;

            // Normalize the direction to get proportional positioning around the bike
            Vector3 normalizedDirection = directionToPlayer.normalized;

            // Calculate target position dynamically around the bike
            Vector3 targetPosition = closestBikeTransform.position + normalizedDirection;// * radius;
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

            /*Vector3 directionToPlayer = camera.transform.position - closestBikeTransform.position;
            directionToPlayer.y = 0;

            Vector3 localDirection = closestBikeTransform.InverseTransformDirection(directionToPlayer);
            float sideSign = Mathf.Sign(localDirection.x);
            float frontBackSign = Mathf.Sign(localDirection.z);

            Vector3 targetPosition = closestBikeTransform.position +
                                     new Vector3(sideSign * .25f, 0.5f, frontBackSign*1);
            transform.position = targetPosition;

            Vector3 lookDirection = camera.transform.position - transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = targetRotation;
            }*/
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

            yield return new WaitUntil(() => Input.touchCount == 0 && !Input.GetMouseButton(0));
            isActivated = false;
        }
    }

}
