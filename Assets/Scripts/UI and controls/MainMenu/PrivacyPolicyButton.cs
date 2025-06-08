using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyButton : MainMenuButton
{
    public override void OnClick()
    {
        warningPopup.SetMessage("Moto Grau MX, desenvolvido pela Historic Games, não coleta, armazena ou" +
            " compartilha qualquer dado pessoal de seus usuários. Por transparência, esteja ciente de que " +
            "este app é composto por serviços de terceiros como AdMob para anúncios. Estes serviços podem " +
            "coletar alguns dados e identificadores para fornecimento de anúncios personalizados.");
        warningPopup.FadeIn();
    }
}
