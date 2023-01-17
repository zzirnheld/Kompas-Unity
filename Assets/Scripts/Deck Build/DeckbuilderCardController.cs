using KompasCore.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasDeckbuilder
{
    public class DeckbuilderCardController : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        protected DeckbuildSearchController cardSearchController;
        protected DeckbuilderController deckbuildCtrl => cardSearchController.DeckbuilderCtrl;

        public Image image;

        public DeckbuilderCard Card { get; private set; }

        public string BlurbString => Card.StatsString + Card.QualifiedSubtypeText;

        public virtual void SetInfo(DeckbuildSearchController searchCtrl, SerializableCard card, bool inDeck, string fileName)
        {
            Card = new DeckbuilderCard(card, fileName);
            cardSearchController = searchCtrl;
            SetImage();
        }

        public void Select()
        {
            cardSearchController.Select(this);
        }

        /// <summary>
        /// Shows this card in the "selected card" area of the deckbuilder
        /// </summary>
        public virtual void Show()
        {
            cardSearchController.deckbuilderCardViewController.Show(Card); //TODO handle compile errors
        }

        protected void SetImage()
        {
            Debug.Log("Setting image");
            image.sprite = Card.SimpleSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                deckbuildCtrl.RemoveFromDeck(this);
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
            deckbuildCtrl.CurrentDrag?.SetIndex(transform.GetSiblingIndex());
        }

        public void SetIndex(int index)
        {
            transform.SetSiblingIndex(index);
            deckbuildCtrl.MoveTo(this, index);
        }

    }

    public class DeckbuilderCard : CardBase
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
}