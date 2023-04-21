using System.Collections.Generic;
using System.IO;
using System.Linq;
using KompasCore.GameCore;
using KompasCore.Helpers;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
	/// <summary>
	/// Controls the segment of the deck building UI that is the actual deck itself
	/// </summary>
	public class DeckPaneDeckController : MonoBehaviour
	{
		public DeckBuilderController deckBuilderController;
		public Transform deckParent;

		/// <summary>
		/// Current decklist that is being shown
		/// </summary>
		private IList<DeckBuilderCardController> currDeck = new List<DeckBuilderCardController>();
		public string CurrDeckName { get; private set; }
		public IEnumerable<string> CurrDeckList => currDeck.Select(card => card.CardName);

		/// <summary>
		/// Map from deck name to "ground truth" decklist. Should be updated when a deck is saved.
		/// </summary>
		private IDictionary<string, IList<string>> deckNameToDeckList = new Dictionary<string, IList<string>>();

		private DeckBuilderCardRepository CardRepo => deckBuilderController.cardRepo;

		/// <summary>
		/// Loads the file for the given <paramref name="deckName"/> and parses out each card name in the deck.
		/// </summary>
		/// <returns>A list of card names present in the deck.</returns>
		public IList<string> Load(string deckName)
		{
			//These have to be string replaces so they can replace with nothing
			var cardNames = DeckLoadHelper.LoadDeck(deckName);
			SetDecklist(deckName, cardNames);
			return cardNames;
		}

		/// <summary>
		/// Used both for saving old decks and creating new ones.
		/// </summary>
		public void SetDecklist(string deckName, IList<string> deckList)
		{
			deckNameToDeckList[deckName] = deckList;
		}

		public void Delete(string deckName)
		{
			deckNameToDeckList.Remove(deckName);
			string filePath = Path.Combine(DeckPaneController.DeckFilesFolderPath, $"{deckName}.txt");
			File.Delete(filePath);
		}

		public void Show(string deckName, bool refresh = false)
		{
			if (CurrDeckName == deckName && !refresh) return;
			ClearDeck();

			CurrDeckName = deckName;
			var cardNames = deckNameToDeckList[CurrDeckName]
				.Where(name => !string.IsNullOrWhiteSpace(name));

			foreach (string name in cardNames) AddToDeck(name);

			currDeck.FirstOrDefault()?.Show();
		}

		public void AddToDeck(string name)
		{
			string json = CardRepo.GetJsonFromName(name);
			if (json == null) return;

			var toAdd = CardRepo.InstantiateDeckBuilderCard(json, deckBuilderController);

			currDeck.Add(toAdd);
			toAdd.gameObject.SetActive(true);
			toAdd.transform.SetParent(deckParent);
			toAdd.transform.localScale = Vector3.one;
		}

		public void ClearDeck()
		{
			if (currDeck == null) return;

			foreach (var card in currDeck) Destroy(card.gameObject);
			currDeck.Clear();
		}

		public void RemoveFromDeck(DeckBuilderCardController card)
		{
			if (!currDeck.Remove(card)) return;

			Destroy(card.gameObject);
		}

		public void ChangeDeckIndex(DeckBuilderCardController card, int index)
		{
			if (!currDeck.Remove(card)) return;

			currDeck.Insert(index, card);

			deckNameToDeckList[CurrDeckName] = currDeck.Select(card => card.CardName).ToArray();
		}
	}
}