using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTargetSubeffect : Subeffect
{
    public CardRestriction cardRestriction;

    /// <summary>
    /// Check if the card passed is a valid target, and if it is, continue the effect
    /// </summary>
    public virtual bool Target(Card card)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (cardRestriction.Evaluate(card))
        {
            parent.targets.Add(card);
            parent.ResolveNextSubeffect();
            Debug.Log("Adding " + card.CardName + " as target");
            return true;
        }

        return false;
    }
}
