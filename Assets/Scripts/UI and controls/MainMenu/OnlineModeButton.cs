using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineModeButton : MainMenuButton
{
    public override void OnClick()
    {
        warningPopup.SetMessage("O modo online custa dinheiro para ser mantido," +
            " portanto só será criado caso o público ativo do jogo seja grande!");
        warningPopup.FadeIn();
    }
}
