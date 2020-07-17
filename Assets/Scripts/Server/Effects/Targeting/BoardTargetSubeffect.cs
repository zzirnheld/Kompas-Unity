using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardTargetSubeffect : CardTargetSubeffect
{
    //there will be a different target effect for board, hand, discard, deck, and combos of these
    
    public CardRestriction cardRestriction;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Initialize(this);
    }

    public override void Resolve()
    {
        //check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (!ThisCard.Game.ExistsCardTarget(cardRestriction))
        {
            Debug.Log("No target exists for " + ThisCard.CardName + " effect");
            ServerEffect.EffectImpossible();
            return;
        }

        //ask the client that is this effect's controller for a target. 
        //give the card if whose effect it is, the index of the effect, and the index of the subeffect
        //since only the server resolves effects, this should never be called for a client.
        EffectController.ServerNotifier.GetBoardTarget(ThisCard, this);

        //then wait for the network controller to call the continue method
    }

    public override bool AddTargetIfLegal(GameCard card)
    {
        Debug.Log("Adding target if legal board target subeff " + card.CardName);
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (cardRestriction.Evaluate(card))
        {
            Debug.Log("Adding " + card.CardName + " as target");
            ServerEffect.AddTarget(card);
            EffectController.ServerNotifier.AcceptTarget();
            ServerEffect.ResolveNextSubeffect();
            return true;
        }
        else
        {
            EffectController.ServerNotifier.GetBoardTarget(ThisCard, this);
        }

        return false;
    }

}
