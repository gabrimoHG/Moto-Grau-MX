using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingsButton : MainMenuButton
{
    public override void OnClick() 
    { 
        warningPopup.SetMessage("Planejo criar rankings com" +
           " os melhores e maiores graus nas próximas versões (em um futuro próximo!).");
        warningPopup.FadeIn();
    }

}
