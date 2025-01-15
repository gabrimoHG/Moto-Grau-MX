using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingsButton : MainMenuButton
{
    public override void OnClick() 
    { 
        warningPopup.SetMessage("Recurso surpresa a ser" +
            " incluído nas próximas versões.");
        warningPopup.FadeIn();
    }

}
