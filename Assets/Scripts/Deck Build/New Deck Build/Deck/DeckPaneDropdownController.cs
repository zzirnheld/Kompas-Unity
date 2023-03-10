using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace KompasDeckbuilder.UI.Deck
{
    /// <summary>
    /// Controls the dropdown for selecting a deck
    /// </summary>
    public class DeckPaneDropdownController : MonoBehaviour
    {
        private static readonly int TXTExtLen = ".txt".Length;

        public TMP_Dropdown dropdown;
        public DeckPaneDeckController deckController;

        public void Load()
        {
            Directory.CreateDirectory(DeckPaneController.DeckFilesFolderPath);

            dropdown.options.Clear();
            var deckNames = new List<string>();

            DirectoryInfo dirInfo = new DirectoryInfo(DeckPaneController.DeckFilesFolderPath);
            FileInfo[] files = dirInfo.GetFiles("*.txt");
            foreach (FileInfo fi in files)
            {
                //add the file name without the ".txt" characters
                string deckName = fi.Name.Substring(0, fi.Name.Length - TXTExtLen);
                if (string.IsNullOrWhiteSpace(deckName)) continue;

                deckNames.Add(deckName);
                var deckList = deckController.Load(deckName);

                //var test = GetAvatarImage(decklist);
                //Debug.Log($"image not null... {test != null}");
                AddDeckListToDropdown(deckName, deckList);
            }

            //load initially selected deck
            dropdown.RefreshShownValue();
            deckController.Show(deckNames[0]);
        }

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

        private Sprite GetAvatarImage(IEnumerable<string> deckList)
        {
            var avatarName = deckList.FirstOrDefault();
            //Debug.Log($"Getting Avatar image for \"{avatarName}\" (len {avatarName.Length}) from {decklist}");

            if (avatarName == null) return null;

            return CardRepository.LoadSprite(CardRepository.FileNameFor(avatarName));
        }

        public void Show(int index) => deckController.Show(dropdown.options[index].text);
    }
}