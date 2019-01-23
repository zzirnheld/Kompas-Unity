﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : KompasObject
{

    List<Card> hand = new List<Card>();

    //rng for shuffling
    private static System.Random rng = new System.Random();

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

    public Card RemoveFromHandAt(int index)
    {
        if (index < 0 || index >= hand.Count) return null;
        Card toReturn = hand[index];
        hand.RemoveAt(index);
        return toReturn;
    }

    public Card RemoveRandomCard()
    {
        int randomIndex = rng.Next(hand.Count);
        Card toReturn = hand[randomIndex];
        RemoveFromHandAt(randomIndex);
        return toReturn;
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
