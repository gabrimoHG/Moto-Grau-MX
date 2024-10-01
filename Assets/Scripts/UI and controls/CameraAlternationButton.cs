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
        button.onClick.AddListener(
            GameManager.instance.cameraController.SwitchCamera);
    }
}
