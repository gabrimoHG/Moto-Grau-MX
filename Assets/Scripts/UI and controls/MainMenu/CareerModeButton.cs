using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerModeButton : MainMenuButton
{
    public override void OnClick()
    {
        warningPopup.SetMessage("O modo carreira está em desenvolvimento...");
        warningPopup.FadeIn();
    }
}
