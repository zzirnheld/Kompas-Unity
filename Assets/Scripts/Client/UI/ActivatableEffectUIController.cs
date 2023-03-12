using KompasCore.Effects;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
    public class ActivatableEffectUIController : MonoBehaviour
    {
        public TMP_Text effNameText;

        private RightClickCardClientUIController parentCtrl;
        private Effect eff;

        public (Effect, RightClickCardClientUIController) Params
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