using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class DeckSelectUIController : MonoBehaviour
{
    public const int txtExtLen = 4;

    public ClientUIController ClientUICtrl;
    public CardRepository CardRepo;
    public GameObject[] CardPrefabs = new GameObject[3];

    //ui elements
    public TMP_Dropdown DeckNameDropdown;
    public GameObject DeckViewScrollPane;
    public TMP_Text CardsInDeckText;

    public List<DeckSelectCard> currDeck;

    private List<string> deckNames;
    private string deckFilesFolderPath;

    // Start is called before the first frame update
    void Start()
    {
        //for now, load an empty list. later, load a default deck?
        currDeck = new List<DeckSelectCard>();
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

    private void SetDeckCountText()
    {
        CardsInDeckText.text = $"Cards in Deck: {currDeck.Count}";
    }

    private void ClearDeck()
    {
        for (int i = currDeck.Count - 1; i >= 0; i--)
        {
            DeckSelectCard c = currDeck[i];
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

        DeckSelectCard toAdd = CardRepo.InstantiateDeckSelectCard(json, DeckViewScrollPane.transform, CardPrefabs);
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
}
