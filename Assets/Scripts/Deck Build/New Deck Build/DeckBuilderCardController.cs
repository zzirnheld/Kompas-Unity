using KompasCore.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasDeckbuilder.UI
{
    public class DeckBuilderCardController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        private class DeckbuilderCard : CardBase
        {
            public override int BaseN { get; }
            public override int BaseE { get; }
            public override int BaseS { get; }
            public override int BaseW { get; }
            public override int BaseC { get; }
            public override int BaseA { get; }

            public DeckbuilderCard(SerializableCard card, string fileName)
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
        
        private DeckBuilderController deckbuildCtrl;

        public Image image;

        private DeckbuilderCard card;
        public string CardName => card.CardName;

        public void SetInfo(SerializableCard card, DeckBuilderController deckbuildCtrl, string fileName)
        {
            this.card = new DeckbuilderCard(card, fileName);
            this.deckbuildCtrl = deckbuildCtrl;
            SetImage();
        }

        /// <summary>
        /// Shows this card in the "selected card" area of the deckbuilder
        /// </summary>
        public void Show() => deckbuildCtrl.cardViewController.Show(card);

        private void SetImage() => image.sprite = card.SimpleSprite;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                deckbuildCtrl.deckPaneController.deckController.RemoveFromDeck(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                deckbuildCtrl.CurrentDrag = this;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            deckbuildCtrl.CurrentDrag = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            deckbuildCtrl.DragEnteredIndex(transform.GetSiblingIndex());
            Show();
        }

    }
}