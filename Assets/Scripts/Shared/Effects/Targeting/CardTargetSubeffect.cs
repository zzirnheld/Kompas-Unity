using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardTargetSubeffect : Subeffect
{
    public CardRestriction cardRestriction;

    public override void Initialize()
    {
        base.Initialize();
        cardRestriction.Subeffect = this;
    }

    /// <summary>
    /// Check if the card passed is a valid target, and if it is, continue the effect
    /// </summary>
    public virtual bool AddTargetIfLegal(Card card)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (cardRestriction.Evaluate(card))
        {
            Effect.targets.Add(card);
            Effect.ResolveNextSubeffect();
            Debug.Log("Adding " + card.CardName + " as target");
            return true;
        }

        return false;
    }
}
