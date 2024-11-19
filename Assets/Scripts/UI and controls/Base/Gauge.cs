using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArcadeBP_Pro;

public class Gauge : MonoBehaviour
{
    protected ArcadeBikeControllerPro bikeController;
    protected TextMeshProUGUI TMProContent;
    protected virtual void Awake()
    {
        TMProContent = GetComponent<TextMeshProUGUI>();
    }
    protected virtual void Start()
    {
        GameManager.instance.onPlayerBikeChange += UpdateBikeController;
        bikeController = GameManager.instance.GetCurrentBikeController();
    }

    public void UpdateBikeController(ArcadeBikeControllerPro bikeController) =>
        this.bikeController = bikeController;
}
