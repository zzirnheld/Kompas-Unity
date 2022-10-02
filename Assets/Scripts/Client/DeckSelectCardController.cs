using KompasCore.Cards;
using KompasCore.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class DeckSelectCardController : ImageOnlyCardViewController, IPointerDownHandler
    {
        public DeckSelectCard Card { get; private set; }
        private DeckSelectUIController uiController;

        public string FileName
        {
            get => Card.FileName;
            set
            {
                Card.FileName = value;
                Refresh();
            }
        }

        public void SetInfo(SerializableCard card, DeckSelectUIController uiCtrl)
        {
            Card = new DeckSelectCard(card);
            uiController = uiCtrl;
            Focus(Card);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            uiController.SelectAsAvatar(this);
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
                       card.subtypeText)
        { }
    }
}
