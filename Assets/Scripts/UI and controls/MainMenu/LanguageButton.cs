using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LanguageButton : MainMenuButton
{
    public override void OnClick()
    {
        warningPopup.SetMessage("Future versions will have other languages!\n\n" +
            "¡Las futuras versiones tendrán otros idiomas!\n\n" +
            "Versi mendatang akan memiliki bahasa lain!\n\n" +
            "Bhavishya mein anya bhashaen hongi!");

        warningPopup.FadeIn();
    }

}
