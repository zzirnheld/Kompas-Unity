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
        public void SetInfo(SerializableCard card, DeckSelectUIController uiCtrl, string fileName)
        {
            Card = new DeckSelectCard(card, fileName);
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
        public DeckSelectCard(SerializableCard card, string fileName)
            : base((card.n, card.e, card.s, card.w, card.c, card.a),
                       card.subtext, card.spellTypes,
                       card.fast, card.unique,
                       card.radius, card.duration,
                       card.cardType, card.cardName, fileName,
                       card.effText,
                       card.subtypeText)
        { }
    }
}
