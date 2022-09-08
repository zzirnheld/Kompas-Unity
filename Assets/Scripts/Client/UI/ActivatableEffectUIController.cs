using KompasCore.Effects;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
    public class ActivatableEffectUIController : MonoBehaviour
    {
        public TMP_Text effNameText;

        private RightClickCardUIController parentCtrl;
        private Effect eff;

        public (Effect, RightClickCardUIController) Params
        {
            set
            {
                (eff, parentCtrl) = value;
                effNameText.text = eff.blurb;
            }
        }

        public void OnClick()
        {
            parentCtrl.clientUICtrl.ActivateCardEff(eff.Source, eff.EffectIndex);
            parentCtrl.Clear();
        }
    }
}