using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KompasDeckbuilder
{
    public class DeckbuilderController : MonoBehaviour
    {
        public const int txtExtLen = 4;
        private const string DeckDeleteFailedErrorMsg = "Failed to delete deck, sorry";

        private string deckFilesFolderPath = "";

        //other ui element controllersS
        public CardRepository CardRepo;
        public CardSearchController CardSearchCtrl;
        public ConfirmDialogController ConfirmDialog;
        public ErrorDialogController ErrorDialog;
        public ImportDeckController ImportDialog;
        public ExportDeckController ExportDialog;

        //ui elements
        public GameObject DeckViewScrollPane;
        public TMP_Dropdown DeckNameDropdown;
        public TMP_Text CardsInDeckText;

        //deck data
        private List<string> deckNames;
        private List<DeckbuilderCard> currDeck;
        private string currDeckName = "";
        private string LastDeletedName = "";
        private bool IsDeckDirty = false;

        public void Start()
        {
            //for now, load an empty list. later, load a default deck?
            currDeck = new List<DeckbuilderCard>();
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
            Debug.Log("Load initially selected deck");
            LoadDeck(0);
        }

        #region actions that need to be confirmed
        public void ToMainMenu()
        {
            if (IsDeckDirty)
            {
                ConfirmDialog.Enable(ConfirmDialogController.ConfirmAction.ToMainMenu);
                return;
            }
            //load the main menu scene
            SceneManager.LoadScene(MainMenuUICtrl.MainMenuScene);
        }

        public void ConfirmToMainMenu()
        {
            //load the main menu scene
            SceneManager.LoadScene(MainMenuUICtrl.MainMenuScene);
        }

        public void ImportDeck()
        {
            if (IsDeckDirty)
            {
                ConfirmDialog.Enable(ConfirmDialogController.ConfirmAction.ImportDeck);
                return;
            }

            ImportDialog.EnableDeckImport();
        }

        public void ConfirmImportDeck()
        {
            ImportDialog.EnableDeckImport();
        }

        public void LoadDeck(string deckName)
        {
            Debug.Log($"Loading {deckName}");

            if (IsDeckDirty)
            {
                Debug.Log($"{deckName} is dirty, showing confirm dialog instead");
                ConfirmDialog.Enable(ConfirmDialogController.ConfirmAction.LoadDeck);
                return;
            }

            //then add new cards
            string filePath = deckFilesFolderPath + "/" + deckName + ".txt";

            string decklist = File.ReadAllText(filePath);
            ImportDeck(decklist, deckName);
        }

        public void LoadDeck(int i)
        {
            if (i >= deckNames.Count)
            {
                Debug.LogError($"Tried to load deck at index {i} out of bounds");
                return;
            }
            LoadDeck(deckNames[i]);
        }

        public void ConfirmLoadDeck()
        {
            IsDeckDirty = false;
            LoadDeck(DeckNameDropdown.value);
        }

        public void CancelLoadDeck()
        {
            int currDeckIndex = deckNames.IndexOf(currDeckName);
            DeckNameDropdown.value = currDeckIndex < 0 ? 0 : currDeckIndex;
        }
        #endregion

        #region helper methods
        private void SetDeckCountText()
        {
            CardsInDeckText.text = $"Cards in Deck: {currDeck.Count}";
        }

        public void ClearDeck()
        {
            Debug.Log("Clearing deck");
            for (int i = currDeck.Count - 1; i >= 0; i--)
            {
                DeckbuilderCard c = currDeck[i];
                currDeck.RemoveAt(i);
                Destroy(c.gameObject);
            }
        }

        private string GetCurrentDeckAsString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (DeckbuilderCard card in currDeck)
            {
                stringBuilder.AppendLine(card.CardName);
            }

            return stringBuilder.ToString();
        }
        #endregion helper methods

        public void ExportDeck()
        {
            ExportDialog.Show(GetCurrentDeckAsString());
        }

        public void AddToDeck(DeckbuilderCard card)
        {
            AddToDeck(card.CardName);
        }

        public void AddToDeck(string name)
        {
            string json = CardRepo.GetJsonFromName(name);
            if (json == null) return;

            DeckbuilderCard toAdd = CardRepo.InstantiateDeckbuilderCard(json, CardSearchCtrl, true);
            if (toAdd == null)
            {
                Debug.LogError($"Somehow have a DeckbuilderCard with name {name} couldn't be re-instantiated");
                return;
            }

            IsDeckDirty = true;
            currDeck.Add(toAdd);
            toAdd.gameObject.SetActive(true);
            toAdd.transform.parent = DeckViewScrollPane.transform;
            toAdd.transform.localScale = Vector3.one;
            SetDeckCountText();
        }

        public void DeleteDeck()
        {
            int index = deckNames.IndexOf(currDeckName);

            if (index != -1)
            {
                Debug.Log($"Deleting {currDeckName}");
                string deckFilePath = deckFilesFolderPath + "/" + currDeckName + ".txt";
                if (File.Exists(deckFilePath))
                {
                    File.Delete(deckFilePath);
                }

                ClearDeck();
                deckNames.RemoveAt(index);
                DeckNameDropdown.options.Clear();

                foreach (string name in deckNames)
                {
                    DeckNameDropdown.options.Add(new TMP_Dropdown.OptionData() { text = name });
                }

                if (deckNames.Count > 0)
                {
                    index = index < deckNames.Count ? index : 0;
                    currDeckName = deckNames[index];
                    DeckNameDropdown.value = index;
                }
                else
                {
                    currDeckName = "";
                }
                DeckNameDropdown.RefreshShownValue();
            }
            else
            {
                ErrorDialog.ShowError(DeckDeleteFailedErrorMsg);
            }
        }

        public void ImportDeck(string decklist, string deckName)
        {
            //first clear deck
            ClearDeck();

            decklist = decklist.Replace("\u200B", "");
            decklist = decklist.Replace("\r", "");
            decklist = decklist.Replace("\t", "");
            List<string> cardNames = new List<string>(decklist.Split('\n'));

            if (deckName == null) deckName = cardNames[0];

            foreach (string name in cardNames)
            {
                if (!string.IsNullOrWhiteSpace(name)) AddToDeck(name);
            }

            currDeckName = deckName;
            IsDeckDirty = false;
            SetDeckCountText();
        }

        public void SaveDeck()
        {
            /*if (string.IsNullOrWhiteSpace(DeckNameInput.text))
            {
                Debug.Log("Tried to save blank deck name, ignoring");
                return;
            }*/

            currDeckName = currDeck.First().CardName;

            //write to a persistent file
            string filePath = deckFilesFolderPath + "/" + currDeckName + ".txt";

            string decklist = GetCurrentDeckAsString();
            Debug.Log($"Saving deck to {filePath}\n{decklist}");
            File.WriteAllText(filePath, decklist);

            IsDeckDirty = false;

            if (!deckNames.Contains(currDeckName))
            {
                deckNames.Add(currDeckName);
                DeckNameDropdown.options.Add(new TMP_Dropdown.OptionData() { text = currDeckName });
                DeckNameDropdown.value = deckNames.Count - 1;
            }
        }

        public void RemoveFromDeck(DeckbuilderCard card)
        {
            if (currDeck.Remove(card))
            {
                IsDeckDirty = true;
                LastDeletedName = card.CardName;
                Destroy(card.gameObject);
            }

            SetDeckCountText();
        }

        public void UndoLastDelete()
        {
            if (string.IsNullOrWhiteSpace(LastDeletedName)) return;
            AddToDeck(LastDeletedName);
        }
    }
}