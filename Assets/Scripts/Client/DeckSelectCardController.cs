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
        public override int BaseN { get; }
        public override int BaseE { get; }
        public override int BaseS { get; }
        public override int BaseW { get; }
        public override int BaseC { get; }
        public override int BaseA { get; }

        public DeckSelectCard(SerializableCard card, string fileName)
            : base((card.n, card.e, card.s, card.w, card.c, card.a),
                       card.subtext, card.spellTypes,
                       card.unique,
                       card.radius, card.duration,
                       card.cardType, card.cardName, fileName,
                       card.effText,
                       card.subtypeText)
        {
            BaseN = card.n;
            BaseE = card.e;
            BaseS = card.s;
            BaseW = card.w;
            BaseC = card.c;
            BaseA = card.a;
        }
    }
}
