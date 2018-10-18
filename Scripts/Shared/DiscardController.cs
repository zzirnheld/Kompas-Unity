using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardController : KompasObject {

    List<Card> discard = new List<Card>();

    //info about discard
    public int DiscardSize() { return discard.Count; }

    public Card CardAt(int index, bool pop)
    {
        if (index >= discard.Count) return null;
        Card card = discard[index];
        if (pop) discard.RemoveAt(index);
        return card;
    }

    //adding/removing cards
	public void AddToDiscard(Card card)
    {
        //TODO
        card.SetLocation(Card.CardLocation.Discard);
        throw new NotImplementedException();
    }

    public void RemoveFromDiscard(Card card)
    {
        //TODO
        throw new NotImplementedException();
    }
}
