using HistoricGamesUtils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FixAndRefuelButton : MonoBehaviour
{
    Image icon;
    TextMeshProUGUI title;
    GameManager gameManager;
    Coroutine checkBikeStatus;
    // Start is called before the first frame update
    void Start()
    {
        title = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        icon = transform.GetChild(1).GetComponent<Image>();
        gameManager = UIManager.instance.gameManager;
        checkBikeStatus = StartCoroutine(CheckBikeStatus());
    }

    IEnumerator CheckBikeStatus()
    {
        while (true)
        {
            var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();
            var isAdAvailable = gameManager.rewardedInterstitialAdController.isAvailable;
            var isAdClicked = gameManager.rewardedInterstitialAdController.isAdClicked;
            if ((bikePhysicsController.condition == 0 ||
                bikePhysicsController.fuelAmount == 0) &&
                isAdAvailable && !isAdClicked)
                EnableButton();
            else
                DisableButton();
            yield return new WaitForSeconds(15);
        }
    }

    private void OnEnable()
    {
        if(checkBikeStatus!=null)
            StartCoroutine(CheckBikeStatus());
    }
    private void OnDisable()
    {
        StopCoroutine(CheckBikeStatus());
    }

    void EnableButton()
    {
        GetComponent<Button>().interactable = true;
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(icon, 1, .25f));
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(title, 1, .25f));
    }

    public void DisableButton()
    {
        GetComponent<Button>().interactable = false;
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(icon,0, .25f));
        StartCoroutine(InterpolationUtilities.AlphaToTargetLinearly(title, 0, .25f));
    }

    public void FixAndRefuel()
    {
        var bikePhysicsController = gameManager.GetCurrentBikePhysicsController();
        bikePhysicsController.Refuel(100);
        bikePhysicsController.Repair(100);
    }
}
