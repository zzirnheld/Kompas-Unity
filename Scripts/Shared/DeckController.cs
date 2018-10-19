using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : KompasObject
{
    //rng for shuffling
    private static System.Random rng = new System.Random();

    //prefabs to instantiate in deck
    public GameObject characterCardPrefab;
    public GameObject spellCardPrefab;
    public GameObject augmentCardPrefab;

    //actual factual deck list
    private List<Card> deck = new List<Card>();


    //importing deck
    public Card InstantiateCard(string json)
    {

        SerializableCharCard serializableChar;
        SerializableSpellCard serializableSpell;
        SerializableCard serializableCard;
        SpellCard spellCard;
        CharacterCard charCard;
        serializableCard = JsonUtility.FromJson<SerializableCard>(json);
        switch (serializableCard.cardType)
        {
            case 'C':
                serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                charCard = Instantiate(characterCardPrefab).GetComponent<CharacterCard>();
                charCard.gameObject.SetActive(false);
                charCard.SetInfo(serializableChar);
                //set image for the card by the name. this method gets the sprite with the given name
                charCard.SetImage(charCard.CardName);
                return charCard;
            case 'S':
                serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                spellCard = Instantiate(spellCardPrefab).GetComponent<SpellCard>();
                spellCard.gameObject.SetActive(false);
                spellCard.SetInfo(serializableSpell);
                //set image for the card by the name. this method gets the sprite with the given name
                spellCard.SetImage(spellCard.CardName);
                return spellCard;
            case 'A':
                serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                spellCard = Instantiate(augmentCardPrefab).GetComponent<AugmentCard>();
                spellCard.gameObject.SetActive(false);
                spellCard.SetInfo(serializableSpell);
                //set image for the card by the name. this method gets the sprite with the given name
                spellCard.SetImage(spellCard.CardName);
                return spellCard;
            default:
                Debug.Log("Unrecognized type character in " + json);
                return null;
        }
    }

    public void ImportDeck(string decklist)
    {
        string[] cards = decklist.Split('\n');
        string folder = "Card Jsons/";
        string path;
        string fileContents = "";
        Card newCard;

        foreach (string card in cards)
        {
            path = folder + card;
            fileContents = Resources.Load<TextAsset>(path).text;

            Debug.Log("Loading:\n" + fileContents);

            newCard = InstantiateCard(fileContents);
            newCard.SetLocation(Card.CardLocation.Deck);
            deck.Add(newCard);
            // end switch
        } //end for each card in the array of cards
    }

    //info about deck
    public int DeckSize() { return deck.Count; }
    
    /// <summary>
    /// Gets the card at the designated index.
    /// </summary>
    /// <param name="index">Index of the card to get</param>
    /// <param name="pop">Whether or not to remove the card</param>
    /// <param name="shuffle">Whether or not to shuffle the deck after getting the card</param>
    /// <returns></returns>
    public Card CardAt(int index, bool pop, bool shuffle = false)
    {
        if (index > deck.Count) return null;
        Card card = deck[index];
        if (pop) deck.RemoveAt(index);
        if (shuffle) Shuffle();
        return card;
    }

    //adding and removing cards
    public void PushTopdeck(Card card)
    {
        //TODO
        card.SetLocation(Card.CardLocation.Deck);
        card.gameObject.SetActive(false); //do this in setlocation?
    }

    public Card PopTopdeck()
    {
        //TODO
        return null;
    }

    /// <summary>
    /// Random access remove from deck
    /// </summary>
    public void RemoveFromDeck(Card card)
    {
        //TODO
        throw new NotImplementedException();
    }

    //misc
    public void Shuffle()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

}
