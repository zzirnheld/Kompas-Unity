using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : KompasObject
{

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
}
