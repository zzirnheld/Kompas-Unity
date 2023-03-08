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
                var decklist = deckController.Load(deckName);

                //var test = GetAvatarImage(decklist);
                //Debug.Log($"image not null... {test != null}");

                dropdown.options.Add(new TMP_Dropdown.OptionData() {
                    text = deckName,
                    image = GetAvatarImage(decklist) 
                });
            }

            //load initially selected deck
            dropdown.RefreshShownValue();
            deckController.Show(deckNames[0]);
        }

        private Sprite GetAvatarImage(IEnumerable<string> decklist)
        {
            var avatarName = decklist.FirstOrDefault();
            //Debug.Log($"Getting Avatar image for \"{avatarName}\" (len {avatarName.Length}) from {decklist}");

            if (avatarName == null) return null;

            return CardRepository.LoadSprite(CardRepository.FileNameFor(avatarName));
        }

        public void Show(int index) => deckController.Show(dropdown.options[index].text);
    }
}