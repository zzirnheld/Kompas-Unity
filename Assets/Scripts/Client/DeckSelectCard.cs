using KompasCore.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class DeckSelectCard : CardBase, IPointerDownHandler
    {
        public DeckSelectUIController UICtrl;
        public Image Image;

        public override string FileName
        {
            get => base.FileName;
            set
            {
                base.FileName = value;
                SetImage();
            }
        }

        public void SetInfo(SerializableCard card, DeckSelectUIController uiCtrl)
        {
            base.SetCardInformation(card);
            UICtrl = uiCtrl;
        }

        protected void SetImage()
        {
            Image.sprite = SimpleSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UICtrl.SelectAsAvatar(this);
        }
    }
}
