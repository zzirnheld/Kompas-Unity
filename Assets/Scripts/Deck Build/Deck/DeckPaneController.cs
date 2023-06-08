using System.Collections.Generic;
using System.IO;
using KompasCore.Helpers;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
	/// <summary>
	/// Controls the middle third of the Deckbuilding UI, which handles the actual deck being built.
	/// </summary>
	public class DeckPaneController : MonoBehaviour
	{
		public static string DeckFilesFolderPath { get; private set; }

		public enum EditMode { Edit, Import, Export }

		[EnumNamedArray(typeof(EditMode))]
		public GameObject[] editModeParents; //Should be in order of the above enum

		public DeckPaneDeckController deckController;
		public DeckPaneDropdownController dropdownController;
		public DeckPaneSaveController saveController;

		public GameObject moreDeckButtonsParent;

		public void Start()
		{
			DeckFilesFolderPath ??= Path.Combine(Application.persistentDataPath, "Decks");
			
			dropdownController.Load();
		}

		public void ShowMoreButtons(bool show)
		{
			moreDeckButtonsParent.SetActive(show);
		}

		public void CreateDeck(string deckName, IList<string> deckList, bool save = false)
		{
			deckController.SetDecklist(deckName, deckList);
			int index = dropdownController.AddDeckListToDropdown(deckName, deckList);
			dropdownController.Select(index);
			if (save) saveController.SaveDeck();
		}

		public void Show (EditMode editMode) => CollectionsHelper.ShowOnly(editModeParents, (int)editMode);

	}
}