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

        public void Awake()
        {
            image = GetComponent<Image>();
        }

        public virtual void SetInfo(DeckbuildSearchController searchCtrl, SerializableCard card, bool inDeck)
        {
            SetInfo(card);
            SetImage(CardName);
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
            cardSearchController.cardViewParentObj.SetActive(true);

            cardSearchController.CardImage.sprite = simpleSprite;
            cardSearchController.CardNameText.text = CardName;
            cardSearchController.SubtypesText.text = QualifiedSubtypeText;
            cardSearchController.EffectText.text = EffText;

            cardSearchController.nText.gameObject.SetActive(CardType == 'C');
            cardSearchController.eText.gameObject.SetActive(CardType == 'C');
            cardSearchController.wText.gameObject.SetActive(CardType == 'C');
            cardSearchController.nText.text = $"N\n{N}";
            cardSearchController.eText.text = $"E\n{E}";
            cardSearchController.wText.text = $"W\n{W}";

            cardSearchController.scaText.text = CardType == 'C' ? $"S\n{S}" :
                                                CardType == 'S' ? $"C\n{C}" :
                                              /*CardType == 'A'*/ $"A\n{A}";

            cardSearchController.ShowReminderText(this);
        }

        public void Unshow()
        {
            cardSearchController.ShowSelectedCard();
        }

        protected void SetImage(string cardFileName)
        {
            simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
            image.sprite = simpleSprite;
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