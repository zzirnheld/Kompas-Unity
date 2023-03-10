using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
    public class DeckPaneSaveController : MonoBehaviour
    {
        public DeckPaneDeckController deckController;
        public DeckPaneDropdownController dropdownController;

        public TMP_InputField deckNameText;
        public GameObject saveAsView;

        public void SaveDeckAs(string deckName)
        {
            //write to a persistent file
            string filePath = Path.Combine(DeckPaneController.DeckFilesFolderPath, $"{deckName}.txt");

            var deckListArr = deckController.CurrDeckList.ToArray();
            string deckList = string.Join("\n", deckListArr);
            Debug.Log($"Saving deck to {filePath}:\n{deckList}");
            File.WriteAllText(filePath, deckList);

            deckController.SetDecklist(deckName, deckListArr);
            int index = dropdownController.AddDeckListToDropdown(deckName, deckListArr);
            dropdownController.Select(index);
        }

        public void SaveDeck() => SaveDeckAs(deckController.CurrDeckName);

        public void ShowSaveAs() => saveAsView.SetActive(true);

        public void HideSaveAs() => saveAsView.SetActive(false);

        public void Confirm()
        {
            SaveDeckAs(deckNameText.text);
            HideSaveAs();
        }

        public void Cancel(){
            deckNameText.text = string.Empty;
            HideSaveAs();
        }
    }
}