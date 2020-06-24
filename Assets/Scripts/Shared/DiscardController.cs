using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardController : MonoBehaviour {

    public Game game;

    private List<GameCard> discard = new List<GameCard>();
    public List<GameCard> Discard { get { return discard; } }

    //info about discard
    public int DiscardSize() { return discard.Count; }
    public GameCard GetLastDiscarded() { return discard[discard.Count - 1]; }

    public GameCard CardAt(int index, bool remove)
    {
        if (index >= discard.Count) return null;
        GameCard card = discard[index];
        if (remove) discard.RemoveAt(index);
        return card;
    }

    //adding/removing cards
	public void AddToDiscard(GameCard card)
    {
        Debug.Assert(card != null);
        Debug.Log("Adding to discard: " + card.CardName);
        discard.Add(card);
        card.SetLocation(CardLocation.Discard);
        card.transform.localPosition = new Vector3(0, 0, (float)discard.Count / -60f);
    }

    public int IndexOf(GameCard card)
    {
        return discard.IndexOf(card);
    }

    public void RemoveFromDiscard(GameCard card)
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
        foreach(GameCard c in discard)
        {
            if (cardRestriction.Evaluate(c)) return true;
        }

        return false;
    }

    public List<GameCard> CardsThatFitRestriction(CardRestriction cardRestriction)
    {
        List<GameCard> cards = new List<GameCard>();

        foreach(GameCard c in discard)
        {
            if (cardRestriction.Evaluate(c)) cards.Add(c);
        }

        return cards;
    }
}
