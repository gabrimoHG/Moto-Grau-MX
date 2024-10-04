using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ArcadeBP_Pro;

public class Gauge : MonoBehaviour
{
    protected ArcadeBikeControllerPro currentBike;
    protected TextMeshProUGUI TMProContent;
    protected virtual void Awake()
    {
        TMProContent = GetComponent<TextMeshProUGUI>();
    }
    protected virtual void Start()
    {
        
    }
}
