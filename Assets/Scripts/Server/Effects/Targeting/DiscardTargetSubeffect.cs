using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardTargetSubeffect : CardTargetSubeffect
{
    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        //check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (!ThisCard.game.ExistsDiscardTarget(cardRestriction, EffectController))
        {
            Debug.Log("No target exists for " + ThisCard.CardName + " effect");
            ServerEffect.EffectImpossible();
            return;
        }

        //ask the client that is this effect's controller for a target. 
        //give the card if whose effect it is, the index of the effect, and the index of the subeffect
        //since only the server resolves effects, this should never be called for a client. 
        EffectController.ServerNotifier.GetDiscardTarget(ThisCard, this);

        //then wait for the network controller to call the continue method
    }

    public override bool AddTargetIfLegal(Card card)
    {
        if (base.AddTargetIfLegal(card)) return true;
        EffectController.ServerNotifier.GetDiscardTarget(ThisCard, this);
        return false;
    }
}
