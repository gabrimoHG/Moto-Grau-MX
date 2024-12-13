using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFirstPCamToggle : OptionToggle
{
    private void Start()
    {
        var cameraController = gameManager.cameraController;
        toggle.isOn = cameraController.IsFirstPersonCamDynamic;
        toggle.onValueChanged.AddListener(
            cameraController.SetIsFirstPersonCamDynamic);
    }
}
