using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardController : MonoBehaviour {

    public Game game;

    private List<Card> discard = new List<Card>();
    public List<Card> Discard { get { return discard; } }

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
        Debug.Log("Adding to discard: " + card.CardName);
        discard.Add(card);
        card.SetLocation(CardLocation.Discard);
        card.transform.localPosition = new Vector3(0, 0, (float)discard.Count / -60f);
    }

    public int IndexOf(Card card)
    {
        return discard.IndexOf(card);
    }

    public void RemoveFromDiscard(Card card)
    {
        Debug.Assert(card != null);
        card.ResetCard();
        discard.Remove(card);
    }

    public void RemoveFromDiscardAt(int index)
    {
        Debug.Assert(index < discard.Count);
        discard.RemoveAt(index);
    }

    public bool Exists(CardRestriction cardRestriction)
    {
        foreach(Card c in discard)
        {
            if (cardRestriction.Evaluate(c)) return true;
        }

        return false;
    }

    public List<Card> CardsThatFitRestriction(CardRestriction cardRestriction)
    {
        List<Card> cards = new List<Card>();

        foreach(Card c in discard)
        {
            if (cardRestriction.Evaluate(c)) cards.Add(c);
        }

        return cards;
    }
}
