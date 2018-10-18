using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardController : KompasObject {

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
