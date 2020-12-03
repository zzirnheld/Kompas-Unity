using KompasCore.Effects;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientUseEffectButtonController : MonoBehaviour
    {
        public const string EffDefaultUIString = "Use Effect";

        public TMPro.TMP_Text buttonText;

        private Effect eff;
        private ClientUIController clientUICtrl;

        public void Initialize(Effect eff, ClientUIController clientUICtrl)
        {
            this.eff = eff;
            buttonText.text = string.IsNullOrEmpty(eff.blurb) ? EffDefaultUIString : eff.blurb;
            this.clientUICtrl = clientUICtrl;
        }

        public void UseEffect()
        {
            clientUICtrl.ActivateSelectedCardEff(eff.EffectIndex);
        }
    }
}