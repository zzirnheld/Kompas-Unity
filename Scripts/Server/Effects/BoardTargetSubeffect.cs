using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardTargetSubeffect : CardTargetSubeffect
{
    //there will be a different target effect for board, hand, discard, deck, and combos of these
    
    public BoardRestriction boardRestriction;

    public override void Resolve()
    {
        boardRestriction.subeffect = this;

        //check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (parent.thisCard.game.NoValidCardOnBoardTarget(boardRestriction))
        {
            Debug.Log("No target exists for " + parent.thisCard.CardName + " effect");
            parent.FinishResolution();
            return;
        }

        //ask the client that is this effect's controller for a target. 
        //give the card if whose effect it is, the index of the effect, and the index of the subeffect
        //since only the server resolves effects, this should never be called for a client.
       parent.serverGame.serverNetworkCtrl
            .GetBoardTarget(parent.serverGame, parent.effectController,  parent.thisCard, 
            System.Array.IndexOf(parent.thisCard.Effects, parent), parent.effectIndex);

        //then wait for the network controller to call the continue method
    }

    public override void Target(Card card)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (boardRestriction.Evaluate(card))
        {
            parent.targets.Add(card);
            parent.ResolveNextSubeffect();
            Debug.Log("Adding " + card.CardName + " as target");
        }
    }

}
