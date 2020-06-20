using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private List<Card> hand = new List<Card>();

    public int HandSize { get { return hand.Count; } }

    //rng for shuffling
    private static System.Random rng = new System.Random();

    public void AddToHand(Card card)
    {
        if (card == null) return;
        hand.Add(card);
        card.ResetCard();
        card.SetLocation(CardLocation.Hand);

        card.transform.rotation = Quaternion.Euler(90, 0, 0);
        SpreadOutCards();
    }

    public int IndexOf(Card card)
    {
        return hand.IndexOf(card);
    }

    public void RemoveFromHand(Card card)
    {
        hand.Remove(card);
        SpreadOutCards();
    }

    public Card RemoveFromHandAt(int index)
    {
        if (index < 0 || index >= hand.Count) return null;
        Card toReturn = hand[index];
        hand.RemoveAt(index);
        SpreadOutCards();
        return toReturn;
    }

    public Card RemoveRandomCard()
    {
        int randomIndex = rng.Next(hand.Count);
        return RemoveFromHandAt(randomIndex);
    }

    public void SpreadOutCards()
    {
        //iterate through children, set the z coord
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.localPosition = new Vector3(((float)hand.Count) + ((float)i * 2f) + 0.5f, 0, 0);
            hand[i].transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public bool Exists(CardRestriction cardRestriction)
    {
        foreach(Card c in hand)
        {
            if (cardRestriction.Evaluate(c)) return true;
        }

        return false;
    }
}
