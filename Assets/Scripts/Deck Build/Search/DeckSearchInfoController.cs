using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasDeckbuilder
{
    public class DeckSearchInfoController : MonoBehaviour
    {
        public Image simpleImage;
        public TMP_Text cardName;
        public TMP_Text cardBlurb;

        private DeckbuilderCardController card;
        private DeckbuildSearchController cardSearchCtrl;

        public void Initialize(DeckbuilderCardController card, DeckbuildSearchController cardSearchCtrl)
        {
            simpleImage.sprite = card.Card.SimpleSprite;
            cardName.text = card.Card.CardName;
            cardBlurb.text = card.BlurbString;
            this.card = card;
            this.cardSearchCtrl = cardSearchCtrl;
        }

        public void OnClick() => cardSearchCtrl.DeckbuilderCtrl.AddToDeck(card.Card);

        public void Kill()
        {
            Destroy(card.gameObject);
        }

        public void Show() => card.Show();
    }
}