using ArcadeBP_Pro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAlternationButton : MonoBehaviour
{
    Button button;
    void Start()
    {
        button = GetComponent<Button>(); 
    }
    public void UpdateSubscription(CameraController cameraController)=>
        button.onClick.AddListener(cameraController.SwitchCamera);
}
