using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ActivationContext
{
    public readonly GameCard Card;
    public readonly IStackable Stackable;
    public readonly Player Triggerer;
    public readonly int? X;
    public readonly (int, int)? Space;
    public readonly int StartIndex;

    public ActivationContext(GameCard card = null, IStackable stackable = null, Player triggerer = null, 
        int? x = null, (int, int)? space = null, int startIndex = 0)
    {
        Card = card;
        Stackable = stackable;
        Triggerer = triggerer;
        X = x;
        Space = space;
        StartIndex = startIndex;
    }
}
