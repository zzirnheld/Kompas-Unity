using KompasCore.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasCore.UI
{
    public class AugmentImageController : MonoBehaviour, IPointerEnterHandler
    {
        private UIController uiCtrl;
        private GameCard card;

        public Image img;

        public void Initialize(GameCard card, UIController uiCtrl)
        {
            this.card = card;
            this.uiCtrl = uiCtrl;
            img.sprite = card.SimpleSprite;
        }

        public void OnPointerEnter(PointerEventData eventData) => uiCtrl.cardViewController.Focus(card);
    }
}