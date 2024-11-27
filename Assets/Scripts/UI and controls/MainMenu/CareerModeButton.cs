using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerModeButton : MainMenuButton
{
    public override void OnClick()
    {
        warningPopup.SetMessage("Modo carreira será implementado" +
            " no futuro caso o projeto cresça!");
        warningPopup.FadeIn();
    }
}
