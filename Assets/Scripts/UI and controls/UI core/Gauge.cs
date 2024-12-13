using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArcadeBP_Pro;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    protected GameManager gameManager;
    protected ArcadeBikeControllerPro bikePhysicsController;
    protected BikeAdditionalPhysics bikeAdditionalPhysics;
    protected TextMeshProUGUI TMProContent;
    protected Image imageContent;
    protected Color lowColor, highColor;
    protected virtual void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();   
        TMProContent = GetComponent<TextMeshProUGUI>();
        imageContent = GetComponent<Image>();
        lowColor = Color.red;
        highColor = Color.green;
    }
    protected virtual void Start()
    {
        bikePhysicsController = FindObjectOfType<GameManager>().GetCurrentBikePhysicsController();
        bikeAdditionalPhysics = bikePhysicsController.GetComponent<BikeAdditionalPhysics>();
        FindObjectOfType<GameManager>().onPlayerBikeChange += UpdateBikeController;//Revisado!
        //GameManager.instance.onBikeReset += UpdateBikeController;
        //Debug.Log($"{name} - Gauge start called at frame {Time.frameCount}");
       
    }

    public void UpdateBikeController(ArcadeBikeControllerPro bikePhysicsController) =>
        this.bikePhysicsController = bikePhysicsController;

    /*public void UpdateBikeController() =>
        bikePhysicsController = GameManager.instance.GetCurrentBikePhysicsController();*/
}
