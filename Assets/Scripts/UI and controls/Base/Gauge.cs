using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gauge : MonoBehaviour
{
    protected TextMeshProUGUI TMProContent;
    protected virtual void Awake()
    {
        TMProContent = GetComponentInChildren<TextMeshProUGUI>();
    }
    protected virtual void Start()
    {
        
    }
}
