using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class DeckbuilderController : MonoBehaviour
{
    private string deckFilesFolderPath = "";

    public CardRespository CardRepo;
    public CardSearchController CardSearchCtrl;
    public GameObject DeckViewScrollPane;
    public TMP_Text DeckNameInput;

    private List<DeckbuilderCard> currDeck;
    private string currDeckName = "";

    public void Awake()
    {
        //for now, load an empty list. later, load a default deck?
        currDeck = new List<DeckbuilderCard>();
        deckFilesFolderPath = Application.persistentDataPath + "/Decks";

        //create the directory if doesn't exist
        Directory.CreateDirectory(deckFilesFolderPath);
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
            Debug.LogError($"Somehow have a DeckbuilderCard with name {name} that doesn't have an assoc. json");
            return;
        }

        DeckbuilderCard toAdd = CardRepo.InstantiateCard(json, DeckViewScrollPane.transform, true);
        if (toAdd == null)
        {
            Debug.LogError($"Somehow have a DeckbuilderCard with name {name} couldn't be re-instantiated");
            return;
        }

        currDeck.Add(toAdd);
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

    public void RemoveFromDeck(DeckbuilderCard card)
    {
        if (currDeck.Remove(card))
        {
            Destroy(card.gameObject);
        }
    }

    public void SaveDeck()
    {
        //TODO temporary, but deck name input for now for testing? ideally a dropdown later
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
    }

    public void LoadDeck(string deckName)
    {
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
    }

    public void LoadDeck()
    {
        LoadDeck(DeckNameInput.text);
    }
}
