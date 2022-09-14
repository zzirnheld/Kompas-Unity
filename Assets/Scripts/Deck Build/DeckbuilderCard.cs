using KompasCore.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasDeckbuilder
{
    public abstract class DeckbuilderCard : CardBase, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        protected DeckbuildSearchController cardSearchController;
        protected DeckbuilderController deckbuildCtrl => cardSearchController.DeckbuilderCtrl;

        protected Image image;

        public abstract string BlurbString { get; }

        public override string FileName
        {
            get => base.FileName;
            set
            {
                base.FileName = value;
                SetImage();
            }
        }

        public void Awake()
        {
            image = GetComponent<Image>();
        }

        public virtual void SetInfo(DeckbuildSearchController searchCtrl, SerializableCard card, bool inDeck)
        {
            SetCardInformation(card);
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
            cardSearchController.deckbuilderCardViewController.Show(this); //TODO handle compile errors
        }

        public void Unshow()
        {
            //TODO
        }

        protected void SetImage()
        {
            image.sprite = SimpleSprite;
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
}