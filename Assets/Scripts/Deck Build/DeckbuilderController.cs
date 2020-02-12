using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckbuilderController : MonoBehaviour
{
    public const int txtExtLen = 4;

    private string deckFilesFolderPath = "";

    public CardRespository CardRepo;
    public CardSearchController CardSearchCtrl;
    public GameObject DeckViewScrollPane;
    public TMP_Dropdown DeckNameDropdown;
    public TMP_InputField DeckNameInput;
    public GameObject ConfirmLoadDeckParentObj;
    public TMP_Text CardsInDeckText;

    private List<string> deckNames;
    private List<DeckbuilderCard> currDeck;
    private string currDeckName = "";
    private string LastDeletedName = "";
    private bool IsDeckDirty = false;

    public void Awake()
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
        foreach(FileInfo fi in files)
        {
            //add the file name without the ".txt" characters
            string deckName = fi.Name.Substring(0, fi.Name.Length - txtExtLen);
            if (string.IsNullOrWhiteSpace(deckName)) continue;
            deckNames.Add(deckName);
            DeckNameDropdown.options.Add(new TMP_Dropdown.OptionData() { text = deckName });
        }

        //load initially selected deck
        LoadDeck(0);
    }

    public void ToMainMenu()
    {
        //load the main menu scene
        SceneManager.LoadScene(MainMenuUICtrl.MainMenuScene);
    }

    private void SetDeckCountText()
    {
        CardsInDeckText.text = $"Cards in Deck: {currDeck.Count}";
    }

    public void AddToDeck(DeckbuilderCard card)
    {
        AddToDeck(card.CardName);
    }

    public void AddToDeck(string name)
    {
        string json = CardRepo.GetJsonFromName(name);
        if (json == null)
        {
            Debug.LogError($"No json found for card name {name}");
            return;
        }

        DeckbuilderCard toAdd = CardRepo.InstantiateCard(json, DeckViewScrollPane.transform, true);
        if (toAdd == null)
        {
            Debug.LogError($"Somehow have a DeckbuilderCard with name {name} couldn't be re-instantiated");
            return;
        }

        IsDeckDirty = true;
        currDeck.Add(toAdd);
        SetDeckCountText();
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

    public void UndoLastDelete()
    {
        if (string.IsNullOrWhiteSpace(LastDeletedName)) return;
        AddToDeck(LastDeletedName);
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

    public void SaveDeck()
    {
        if(string.IsNullOrWhiteSpace(DeckNameInput.text))
        {
            Debug.Log("Tried to save blank deck name, ignoring");
            return;
        }

        currDeckName = DeckNameInput.text;

        //write to a persistent file
        string filePath =  deckFilesFolderPath + "/" + currDeckName + ".txt";

        StringBuilder stringBuilder = new StringBuilder();

        foreach(DeckbuilderCard card in currDeck)
        {
            stringBuilder.AppendLine(card.CardName);
        }

        Debug.Log($"Saving deck to {filePath}\n{stringBuilder.ToString()}");

        File.WriteAllText(filePath, stringBuilder.ToString());

        if (!deckNames.Contains(currDeckName))
        {
            deckNames.Add(currDeckName);
            DeckNameDropdown.options.Add(new TMP_Dropdown.OptionData() { text = currDeckName });
            DeckNameDropdown.value = deckNames.Count - 1;
        }

        IsDeckDirty = false;
    }

    public void LoadDeck(string deckName)
    {
        if (IsDeckDirty)
        {
            ConfirmLoadDeckParentObj.SetActive(true);
            return;
        }

        //first clear deck
        ClearDeck();

        //then add new cards
        string filePath = deckFilesFolderPath + "/" + deckName + ".txt";

        string decklist = File.ReadAllText(filePath);
        decklist = decklist.Replace("\r", "");
        decklist = decklist.Replace("\t", "");
        List<string> cardNames = new List<string>(decklist.Split('\n'));

        foreach (string name in cardNames)
        {
            if(!string.IsNullOrWhiteSpace(name)) AddToDeck(name);
        }

        currDeckName = deckName;
        IsDeckDirty = false;
        DeckNameInput.text = deckName;
        SetDeckCountText();
    }

    public void ConfirmLoadDeck()
    {
        IsDeckDirty = false;
        ConfirmLoadDeckParentObj.SetActive(false);
        LoadDeck(DeckNameDropdown.value);
    }

    public void CancelLoadDeck()
    {
        int currDeckIndex = deckNames.IndexOf(currDeckName);
        DeckNameDropdown.value = currDeckIndex < 0 ? 0 : currDeckIndex;
        ConfirmLoadDeckParentObj.SetActive(false);
    }

    public void LoadDeck(int i)
    {
        if (i >= deckNames.Count) {
            Debug.LogError($"Tried to load deck at index {i} out of bounds");
            return; 
        }
        LoadDeck(deckNames[i]);
    }
}
