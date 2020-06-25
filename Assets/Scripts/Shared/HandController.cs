using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HandController : MonoBehaviour
{
    public Player Owner;

    private List<GameCard> hand = new List<GameCard>();

    public int HandSize { get { return hand.Count; } }

    //rng for shuffling
    private static System.Random rng = new System.Random();

    public virtual void AddToHand(GameCard card, IStackable stackSrc = null)
    {
        if (card == null) return;
        card.Remove();
        hand.Add(card);
        card.ResetCard();
        card.Location = CardLocation.Hand;
        card.Controller = Owner;

        card.transform.rotation = Quaternion.Euler(90, 0, 0);
        SpreadOutCards();
    }

    public int IndexOf(GameCard card)
    {
        return hand.IndexOf(card);
    }

    public void RemoveFromHand(GameCard card)
    {
        hand.Remove(card);
        SpreadOutCards();
    }

    public GameCard RemoveFromHandAt(int index)
    {
        if (index < 0 || index >= hand.Count) return null;
        GameCard toReturn = hand[index];
        hand.RemoveAt(index);
        SpreadOutCards();
        return toReturn;
    }

    public GameCard RemoveRandomCard()
    {
        int randomIndex = rng.Next(hand.Count);
        return RemoveFromHandAt(randomIndex);
    }

    public void SpreadOutCards()
    {
        //iterate through children, set the z coord
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.localPosition = new Vector3((-0.75f * (float)hand.Count) + ((float)i * 2f) + 0.5f, 0, 0);
            hand[i].transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public bool Exists(CardRestriction cardRestriction)
    {
        foreach(GameCard c in hand)
        {
            if (cardRestriction.Evaluate(c)) return true;
        }

        return false;
    }
}
