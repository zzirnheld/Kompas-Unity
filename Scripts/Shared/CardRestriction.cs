using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardRestriction : Restriction
{
    public int costsLessThan;

    public virtual void Evaluate(Card potentialTarget)
    {
        
    }
}
