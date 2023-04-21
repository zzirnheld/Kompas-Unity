using System.Collections.Generic;
using System.IO;
using System.Linq;
using KompasCore.UI;
using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
	/// <summary>
	/// Controls the dropdown for selecting a deck
	/// </summary>
	public class DeckPaneDropdownController : DeckDropdownControllerBase
	{
		public DeckPaneDeckController deckController;

		protected override IList<string> LoadDeck(string deckName)
		{
			return deckController.Load(deckName);
		}

		protected override void Show(string deckName)
		{
			deckController.Show(deckName);
		}

		public void RemoveFromDropdown(string deckName)
		{
			var alreadyThere = dropdown.options.FirstOrDefault(option => option.text == deckName);
			if (alreadyThere == null) return;

			dropdown.options.Remove(alreadyThere);
			Select(0);
			dropdown.RefreshShownValue();
		}

		public void Select(int index)
		{
			dropdown.value = index;
			Show(index);
		}

		public void Show(int index) => deckController.Show(dropdown.options[index].text);
	}
}