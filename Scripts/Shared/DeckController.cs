using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : KompasObject
{
    private static System.Random rng = new System.Random();

    List<Card> deck = new List<Card>();

    public void ImportDeck(string decklist)
    {
        //TODO
        throw new NotImplementedException();
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
