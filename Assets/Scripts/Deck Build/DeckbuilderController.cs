using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DeckbuilderController : MonoBehaviour
{
    private string deckFilesFolderPath = Application.persistentDataPath + "/Decks/";

    public GameObject DeckViewScrollPane;

    private List<DeckbuilderCard> currDeck;
    private string currDeckName = "";

    public void Awake()
    {
        //for now, load an empty list. later, load a default deck?
        currDeck = new List<DeckbuilderCard>();
    }

    public void AddToDeck(DeckbuilderCard card)
    {
        currDeck.Add(card);
        card.transform.SetParent(DeckViewScrollPane.transform);
    }

    public void SaveDeck()
    {
        //write to a persistent file
        string filePath =  deckFilesFolderPath + currDeckName + ".txt";

        StringBuilder stringBuilder = new StringBuilder();

        foreach(DeckbuilderCard card in currDeck)
        {
            stringBuilder.AppendLine(card.CardName);
        }

        File.WriteAllText(deckFilesFolderPath, stringBuilder.ToString());
    }

    public void LoadDeck(string deckName)
    {

    }
}
