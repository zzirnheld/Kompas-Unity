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

        protected Image image;

        public DeckbuilderCard Card { get; private set; }

        public string BlurbString => Card.StatsString + Card.QualifiedSubtypeText;

        public string FileName
        {
            get => Card.FileName;
            set
            {
                Card.FileName = value;
                SetImage();
            }
        }

        public void Awake()
        {
            image = GetComponent<Image>();
        }

        public virtual void SetInfo(DeckbuildSearchController searchCtrl, SerializableCard card, bool inDeck)
        {
            Card = new DeckbuilderCard(card);
            cardSearchController = searchCtrl;
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
        public DeckbuilderCard(SerializableCard card)
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