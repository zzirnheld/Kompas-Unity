using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientUseEffectButtonController : MonoBehaviour
{
    public TMPro.TMP_Text buttonText;

    private Effect eff;
    private ClientUIController clientUICtrl;

    public void Initialize(Effect eff, ClientUIController clientUICtrl)
    {
        this.eff = eff;
        buttonText.text = eff.Blurb;
        this.clientUICtrl = clientUICtrl;
    }

    public void UseEffect()
    {
        clientUICtrl.ActivateSelectedCardEff(eff.EffectIndex);
    }
}
