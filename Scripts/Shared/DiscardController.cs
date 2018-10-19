using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardController : KompasObject {

    List<Card> discard = new List<Card>();

    //info about discard
    public int DiscardSize() { return discard.Count; }
    public Card GetLastDiscarded() { return discard[discard.Count - 1]; }

    public Card CardAt(int index, bool remove)
    {
        if (index >= discard.Count) return null;
        Card card = discard[index];
        if (remove) discard.RemoveAt(index);
        return card;
    }

    //adding/removing cards
	public void AddToDiscard(Card card)
    {
        Debug.Assert(card != null);
        discard.Add(card);
        card.SetLocation(Card.CardLocation.Discard);
        card.transform.localPosition = new Vector3(0, 0, (float)discard.Count / -60f);
    }

    public void RemoveFromDiscard(Card card)
    {
        Debug.Assert(card != null);
        discard.Remove(card);
    }
}
