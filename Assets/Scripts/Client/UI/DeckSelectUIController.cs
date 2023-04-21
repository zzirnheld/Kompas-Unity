using KompasClient.Cards;
using KompasClient.Networking;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

namespace KompasClient.UI
{
	public class DeckSelectUIController : MonoBehaviour
	{
		public const int txtExtLen = 4;

		public ClientNotifier ClientNotifier;
		public ClientCardRepository CardRepo;
		public DeckSelectCardController CardPrefab;

		//ui elements
		public TMP_Dropdown DeckNameDropdown;
		public GameObject DeckViewScrollPane;
		public TMP_Text CardsInDeckText;

		public List<DeckSelectCardController> currDeck;

		private List<string> deckNames;
		private string deckFilesFolderPath;

		// Start is called before the first frame update
		void Start()
		{
			//for now, load an empty list. later, load a default deck?
			currDeck = new List<DeckSelectCardController>();
			deckFilesFolderPath = Application.persistentDataPath + "/Decks";

			//create the directory if doesn't exist
			Directory.CreateDirectory(deckFilesFolderPath);

			//open the deck directory and add all text files to a decklist dropdown
			DeckNameDropdown.options.Clear();
			deckNames = new List<string>();
			DirectoryInfo dirInfo = new DirectoryInfo(deckFilesFolderPath);
			FileInfo[] files = dirInfo.GetFiles("*.txt");
			foreach (FileInfo fi in files)
			{
				//add the file name without the ".txt" characters
				string deckName = fi.Name.Substring(0, fi.Name.Length - txtExtLen);
				if (string.IsNullOrWhiteSpace(deckName)) continue;
				deckNames.Add(deckName);
				DeckNameDropdown.options.Add(new TMP_Dropdown.OptionData() { text = deckName });
			}

			//load initially selected deck
			DeckNameDropdown.RefreshShownValue();
			LoadDeck(0);
		}

		private void SetDeckCountText()
		{
			CardsInDeckText.text = $"Cards in Deck: {currDeck.Count}";
		}

		private void ClearDeck()
		{
			for (int i = currDeck.Count - 1; i >= 0; i--)
			{
				DeckSelectCardController c = currDeck[i];
				currDeck.RemoveAt(i);
				Destroy(c.gameObject);
			}
		}

		private void AddToDeck(string name)
		{
			string json = CardRepo.GetJsonFromName(name);
			if (json == null)
			{
				Debug.LogError($"No json found for card name {name}");
				return;
			}

			DeckSelectCardController toAdd = CardRepo.InstantiateDeckSelectCard(json, DeckViewScrollPane.transform, CardPrefab, this);
			if (toAdd == null)
			{
				Debug.LogError($"Somehow have a DeckbuilderCard with name {name} couldn't be re-instantiated");
				return;
			}

			currDeck.Add(toAdd);
			SetDeckCountText();
		}

		public void LoadDeck(int index)
		{
			if (index > deckNames.Count) return;
			Debug.Log($"Loading {deckNames[index]}");

			//then add new cards
			string filePath = deckFilesFolderPath + "/" + deckNames[index] + ".txt";
			string decklist = File.ReadAllText(filePath);
			LoadDeck(decklist, deckNames[index]);
		}

		public void LoadDeck(string decklist, string deckName)
		{
			//first clear deck
			ClearDeck();

			Debug.Log($"Loading decklist: {decklist}");

			decklist = decklist.Replace("\u200B", "");
			decklist = decklist.Replace("\r", "");
			decklist = decklist.Replace("\t", "");
			List<string> cardNames = new List<string>(decklist.Split('\n'));

			if (deckName == null) deckName = cardNames[0];

			foreach (string name in cardNames)
			{
				if (!string.IsNullOrWhiteSpace(name)) AddToDeck(name);
			}

			SetDeckCountText();
			DeckNameDropdown.RefreshShownValue();
		}

		public void ConfirmSelectedDeck()
		{
			StringBuilder sb = new StringBuilder();
			foreach (DeckSelectCardController card in currDeck)
			{
				sb.Append(card.Card.CardName);
				sb.Append("\n");
			}

			ClientNotifier.RequestDecklistImport(sb.ToString());
		}

		public void SelectAsAvatar(DeckSelectCardController card)
		{
			if (card.Card.CardType != 'C' || !currDeck.Contains(card)) return;

			currDeck.Remove(card);
			currDeck.Insert(0, card);

			//then move it in the ui
			card.transform.SetAsFirstSibling();
		}
	}
}