using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : KompasObject
{

    List<Card> hand = new List<Card>();

    public void AddToHand(Card card)
    {
        hand.Add(card);
        card.SetLocation(Card.CardLocation.Hand);

        card.transform.rotation = Quaternion.Euler(90, 0, 0);
        SpreadOutCards();
    }

    public void RemoveFromHand(Card card)
    {
        hand.Remove(card);
    }

    public void RemoveFromHandAt(int index)
    {
        if (index < 0 || index >= hand.Count) return;
        hand.RemoveAt(index);
    }

    public void SpreadOutCards()
    {
        //iterate through children, set the z coord
        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.localPosition = new Vector3((-0.5f * ((float)hand.Count)) + ((float)i) + 0.5f, 0, 0);
        }
    }
}
