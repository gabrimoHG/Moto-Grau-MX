using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlutuantDirectionalToggle : OptionToggle
{
    private void Start()
    {
        var directionalHandler = UIManager.instance.directionalController;
        toggle.isOn = directionalHandler.FixedDirectional;
        toggle.onValueChanged.AddListener(
            directionalHandler.SetIsFixedDirectional);
    }
}
