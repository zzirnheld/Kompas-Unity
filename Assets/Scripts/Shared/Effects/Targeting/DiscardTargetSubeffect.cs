using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardTargetSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        cardRestriction.subeffect = this;

        //check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (!parent.thisCard.game.ExistsDiscardTarget(cardRestriction, parent.effectControllerIndex))
        {
            Debug.Log("No target exists for " + parent.thisCard.CardName + " effect");
            parent.EffectImpossible();
            return;
        }

        //ask the client that is this effect's controller for a target. 
        //give the card if whose effect it is, the index of the effect, and the index of the subeffect
        //since only the server resolves effects, this should never be called for a client. 
        parent.EffectController.ServerNotifier.GetDiscardTarget(parent.thisCard, parent.EffectIndex, parent.subeffectIndex);

        //then wait for the network controller to call the continue method
    }

    public override bool AddTargetIfLegal(Card card)
    {
        if (base.AddTargetIfLegal(card)) return true;
        parent.EffectController.ServerNotifier.GetDiscardTarget(parent.thisCard, parent.EffectIndex, parent.subeffectIndex);
        return false;
    }
}
