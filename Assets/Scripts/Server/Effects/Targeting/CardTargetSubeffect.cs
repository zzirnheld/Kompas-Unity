using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardTargetSubeffect : ServerSubeffect
{
    public CardRestriction cardRestriction = new CardRestriction();

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
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
            ServerEffect.Targets.Add(card);
            EffectController.ServerNotifier.AcceptTarget();
            ServerEffect.ResolveNextSubeffect();
            Debug.Log("Adding " + card.CardName + " as target");
            return true;
        }

        return false;
    }
}
