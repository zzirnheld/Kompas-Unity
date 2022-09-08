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

        private DeckbuilderCard card;
        private DeckbuildSearchController cardSearchCtrl;

        public void Initialize(DeckbuilderCard card, DeckbuildSearchController cardSearchCtrl)
        {
            simpleImage.sprite = card.SimpleSprite;
            cardName.text = card.CardName;
            cardBlurb.text = card.BlurbString;
            this.card = card;
            this.cardSearchCtrl = cardSearchCtrl;
        }

        public void OnClick() => cardSearchCtrl.DeckbuilderCtrl.AddToDeck(card);

        public void Kill()
        {
            Destroy(card.gameObject);
        }

        public void Show() => card.Show();

        public void Unshow() => card.Unshow();
    }
}