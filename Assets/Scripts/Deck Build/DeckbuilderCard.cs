using KompasCore.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace KompasDeckbuilder
{
    public abstract class DeckbuilderCard : CardBase
    {
        protected CardSearchController cardSearchController;

        protected Image image;
        private bool InDeck;

        public void Awake()
        {
            image = GetComponent<Image>();
        }

        public virtual void SetInfo(CardSearchController searchCtrl, SerializableCard card, bool inDeck)
        {
            SetInfo(card);
            SetImage(CardName);
            InDeck = inDeck;
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
            cardSearchController.CardImage.sprite = detailedSprite;
            cardSearchController.CardNameText.text = CardName;
            cardSearchController.SubtypesText.text = SubtypeText;
            cardSearchController.EffectText.text = EffText;
        }

        public void Unshow()
        {
            cardSearchController.ShowSelectedCard();
        }

        protected void SetImage(string cardFileName)
        {
            detailedSprite = Resources.Load<Sprite>("Detailed Sprites/" + cardFileName);
            simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
            //check if either is null. if so, log to debug and return
            if (detailedSprite == null || simpleSprite == null)
            {
                Debug.LogError("Could not find sprite with name " + cardFileName);
                return;
            }
            image.sprite = simpleSprite;
        }

        public void OnClick()
        {
            Debug.Log($"Clicked {CardName}, in deck? {InDeck}");
            if (InDeck)
            {
                cardSearchController.DeckbuilderCtrl.RemoveFromDeck(this);
            }
            else
            {
                Select();
                cardSearchController.DeckbuilderCtrl.AddToDeck(this);
            }
        }
    }
}