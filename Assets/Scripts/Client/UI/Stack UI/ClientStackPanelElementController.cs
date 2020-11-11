using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class ClientStackPanelElementController : MonoBehaviour
    {
        public Image image;
        public TMP_Text cardNameText;
        public TMP_Text blurbText;

        public void Initialize(Sprite image, string cardName, string blurb)
        {
            if(image != null) this.image.sprite = image;
            cardNameText.text = cardName;
            blurbText.text = blurb;
        }
    }
}