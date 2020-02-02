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
        currDeck.Add(card);
        card.transform.SetParent(DeckViewScrollPane.transform);
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
        string filePath = deckFilesFolderPath + "/" + deckName + ".txt";

        string decklist = File.ReadAllText(filePath);
        List<string> cardNames = new List<string>(decklist.Split());
        List<string> jsons = CardRepo.GetJsonsFromNameList(cardNames);

        foreach (string json in jsons)
        {
            DeckbuilderCard newCard = CardRepo.InstantiateCard(json, DeckViewScrollPane.transform);
            if (newCard != null) AddToDeck(newCard);
        }
    }

    public void LoadDeck()
    {
        LoadDeck(DeckNameInput.text);
    }
}
