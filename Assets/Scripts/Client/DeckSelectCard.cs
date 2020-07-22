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

        public void SetInfo(SerializableCard card, DeckSelectUIController uiCtrl)
        {
            base.SetInfo(card);
            UICtrl = uiCtrl;
            SetImage(CardName);
        }

        protected void SetImage(string cardFileName)
        {
            detailedSprite = Resources.Load<Sprite>("Detailed Sprites/" + cardFileName);
            simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
            //check if either is null. if so, log to debug and return
            if (detailedSprite == null || simpleSprite == null)
            {
                Debug.LogError("Could not find sprite with name " + cardFileName);
                return;
            }
            Image.sprite = simpleSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UICtrl.SelectAsAvatar(this);
        }
    }
}
