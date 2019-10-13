using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardTargetSubeffect : CardTargetSubeffect
{
    //there will be a different target effect for board, hand, discard, deck, and combos of these
    
    public BoardRestriction boardRestriction;

    public override void Initialize()
    {
        base.Initialize();
        boardRestriction.subeffect = this;
    }

    public override void Resolve()
    {
        boardRestriction.subeffect = this;

        //check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (parent.thisCard.game.NoValidCardOnBoardTarget(boardRestriction))
        {
            Debug.Log("No target exists for " + parent.thisCard.CardName + " effect");
            parent.EffectImpossible();
            return;
        }

        //ask the client that is this effect's controller for a target. 
        //give the card if whose effect it is, the index of the effect, and the index of the subeffect
        //since only the server resolves effects, this should never be called for a client.
       ServerGame.serverNotifier.GetBoardTarget(parent.EffectController, parent.thisCard, parent.EffectIndex, parent.subeffectIndex);

        //then wait for the network controller to call the continue method
    }

    public override bool AddTargetIfLegal(Card card)
    {
        Debug.Log("Adding target if legal board target subeff " + card.CardName);
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (boardRestriction.Evaluate(card))
        {
            parent.targets.Add(card);
            parent.ResolveNextSubeffect();
            Debug.Log("Adding " + card.CardName + " as target");
            return true;
        }
        else
        {
            ServerGame.serverNotifier.GetBoardTarget(parent.EffectController, parent.thisCard, parent.EffectIndex, parent.subeffectIndex);
        }

        return false;
    }

}
