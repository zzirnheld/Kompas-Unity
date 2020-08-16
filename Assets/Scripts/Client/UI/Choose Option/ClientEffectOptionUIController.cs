using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientEffectOptionUIController : MonoBehaviour
    {
        public TMP_Text blurbText;

        private ClientChooseOptionUIController ctrl;
        private int index;

        public void Initialize(ClientChooseOptionUIController ctrl, string blurb, int index)
        {
            this.ctrl = ctrl;
            this.index = index;
            blurbText.text = blurb;
        }

        public void OnClick() => ctrl.ChooseOption(index);
    }
}