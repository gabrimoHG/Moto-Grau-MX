using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineModeButton : MainMenuButton
{
    public override void OnClick()
    {
        warningPopup.SetMessage("Projetei o jogo para ter modo online," +
            " porém preciso de mais jogadores para conseguir contratar servidores!");
        warningPopup.FadeIn();
    }
}
