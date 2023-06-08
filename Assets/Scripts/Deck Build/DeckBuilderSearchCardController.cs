using KompasDeckbuilder.UI.Deck;
using UnityEngine;
using UnityEngine.UI;

namespace KompasDeckbuilder.UI.Search
{
	public class DeckBuilderSearchCardController : MonoBehaviour
	{
		public Image image;

		private DeckBuilderCardController card;
		private DeckPaneDeckController deckController;

		public void Initialize(DeckBuilderCardController card, DeckPaneDeckController deckController)
		{
			image.sprite = card.CardSprite;
			this.card = card;
			this.deckController = deckController;
		}

		public void OnDestroy() { if (card != null) Destroy(card.gameObject); }

		public void Show() => card.Show();

		public void AddToDeck() => deckController.AddToDeck(card.CardName);
	}
}