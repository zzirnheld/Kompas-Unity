using KompasCore.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class DeckSelectCardController : MonoBehaviour, IPointerDownHandler
    {
        public DeckSelectUIController UICtrl;
        public Image Image;

        public DeckSelectCard Card { get; private set; }

        public string FileName
        {
            get => Card.FileName;
            set
            {
                Card.FileName = value;
                SetImage();
            }
        }

        public void SetInfo(SerializableCard card, DeckSelectUIController uiCtrl)
        {
            Card = new DeckSelectCard(card);
            UICtrl = uiCtrl;
        }

        protected void SetImage()
        {
            Image.sprite = Card.SimpleSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UICtrl.SelectAsAvatar(this);
        }
    }

    public class DeckSelectCard : CardBase
    {
        public DeckSelectCard(SerializableCard card)
            : base((card.n, card.e, card.s, card.w, card.c, card.a),
                       card.subtext, card.spellTypes,
                       card.fast, card.unique,
                       card.radius, card.duration,
                       card.cardType, card.cardName,
                       card.effText,
                       card.subtypeText,
                       card.augSubtypes)
        {
        }
    }
}
