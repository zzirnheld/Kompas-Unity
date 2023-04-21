using System.Collections.Generic;
using System.IO;
using System.Linq;
using KompasCore.GameCore;
using KompasCore.Helpers;
using KompasDeckbuilder.UI.Deck;
using TMPro;
using UnityEngine;

namespace KompasCore.UI
{
	/// <summary>
	/// Base class for shared logic between deck build and deck select UI controllers
	/// </summary>
	public abstract class DeckDropdownControllerBase : MonoBehaviour
	{
		private static readonly int TXTExtLen = ".txt".Length;

		public TMP_Dropdown dropdown;

		public void Load()
		{
			Directory.CreateDirectory(DeckLoadHelper.DeckFilesFolderPath);

			dropdown.options.Clear();
			var deckNames = new List<string>();

			DirectoryInfo dirInfo = new DirectoryInfo(DeckLoadHelper.DeckFilesFolderPath);
			FileInfo[] files = dirInfo.GetFiles("*.txt");
			foreach (FileInfo fi in files)
			{
				//add the file name without the ".txt" characters
				string deckName = fi.Name.Substring(0, fi.Name.Length - TXTExtLen);
				if (string.IsNullOrWhiteSpace(deckName)) continue;

				deckNames.Add(deckName);
				var deckList = LoadDeck(deckName);

				//var test = GetAvatarImage(decklist);
				//Debug.Log($"image not null... {test != null}");
				AddDeckListToDropdown(deckName, deckList);
			}

			//load initially selected deck
			dropdown.RefreshShownValue();
			Show(deckNames[0]);
		}

		/// <summary>
		/// Should be overridden if you want to do anything else at the moment the deck list is loaded in.
		/// </summary>
		protected virtual IList<string> LoadDeck(string deckName) => DeckLoadHelper.LoadDeck(deckName);
		protected abstract void Show(string deckName);


		public int AddDeckListToDropdown(string deckName, IEnumerable<string> deckList)
		{
			var alreadyThere = dropdown.options.FirstOrDefault(option => option.text == deckName);

			if (alreadyThere != null)
			{
				alreadyThere.image = GetAvatarImage(deckList);
				dropdown.RefreshShownValue();
				return dropdown.options.IndexOf(alreadyThere);
			}

			dropdown.options.Add(new TMP_Dropdown.OptionData() {
				text = deckName,
				image = GetAvatarImage(deckList) 
			});
			return dropdown.options.Count - 1;
		}

		private Sprite GetAvatarImage(IEnumerable<string> deckList)
		{
			var avatarName = deckList.FirstOrDefault();
			if (avatarName == null) return null;

			return CardRepository.LoadSprite(CardRepository.FileNameFor(avatarName));
		}

	}
}