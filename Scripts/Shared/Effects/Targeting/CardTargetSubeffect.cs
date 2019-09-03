using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardTargetSubeffect : Subeffect
{
    public CardRestriction cardRestriction;

    public override void Initialize()
    {
        base.Initialize();
        cardRestriction.subeffect = this;
    }

    /// <summary>
    /// Check if the card passed is a valid target, and if it is, continue the effect
    /// </summary>
    public virtual bool AddTargetIfLegal(Card card)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (cardRestriction.Evaluate(card))
        {
            parent.targets.Add(card);
            parent.serverGame.serverNetworkCtrl.AcceptTarget(parent.serverGame, card, parent.EffectController.ConnectionID);
            parent.ResolveNextSubeffect();
            Debug.Log("Adding " + card.CardName + " as target");
            return true;
        }

        return false;
    }
}
