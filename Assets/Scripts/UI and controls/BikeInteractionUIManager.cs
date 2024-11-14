using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BikeInteractionUIManager : MonoBehaviour
{
    Canvas canvas;
    Transform enterBikeButton;
    BikeSpawner bikeSpawner;
    private void Awake()
    {
        bikeSpawner = transform.parent.GetComponent<BikeSpawner>();
        canvas = GetComponent<Canvas>();
        enterBikeButton = transform.GetChild(0);
        canvas.worldCamera = Camera.main;
    }

    private void OnEnable()
    {
        StartCoroutine(RotateButtonTowardsPlayer());
        StartCoroutine(EnableButtonVisibility());
    }

    void OnDisable()=>StopAllCoroutines();
    IEnumerator EnableButtonVisibility()
    {
        var image = enterBikeButton.GetComponent<Image>();
        while (true)
        {
            if (bikeSpawner.closestDistance <= 5)
            {
                if (image.color.a < 1)
                    StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(image, 1, .5f));
            }
            else
            {
                if (image.color.a > 0)
                    StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(image, 0, .5f));
            }

            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator RotateButtonTowardsPlayer()
    {
        while (true)
        {
            Vector3 directionToPlayer = canvas.worldCamera.transform.position - enterBikeButton.position;
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = targetRotation;
            }

            yield return new WaitForSeconds(.1f);
        }
    }
}
