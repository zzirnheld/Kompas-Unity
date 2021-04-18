using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ClientStackPanelElementController : MonoBehaviour
    {
        public Image primaryImage;
        public Image secondaryImage;
        public TMP_Text blurbText;

        public void Initialize(Sprite primary, Sprite secondary, string blurb)
        {
            if(primary != null) primaryImage.sprite = primary;
            primaryImage.gameObject.SetActive(primary != null);

            if(secondary != null) secondaryImage.sprite = secondary;
            secondaryImage.gameObject.SetActive(secondary != null);

            blurbText.text = blurb;
        }
    }
}