using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckTargetSubeffect : CardTargetSubeffect
{
    public override bool Resolve()
    {
        //check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (!ThisCard.Game.ExistsCardTarget(cardRestriction))
        {
            Debug.Log("No target exists for " + ThisCard.CardName + " effect");
            return ServerEffect.EffectImpossible();
        }

        //ask the client that is this effect's controller for a target. 
        //give the card if whose effect it is, the index of the effect, and the index of the subeffect
        //since only the server resolves effects, this should never be called for a client. 
        EffectController.ServerNotifier.GetDeckTarget(ThisCard, this);

        //then wait for the network controller to call the continue method
        return false;
    }

    public override bool AddTargetIfLegal(GameCard card)
    {
        //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
        if (cardRestriction.Evaluate(card))
        {
            Debug.Log("Adding " + card.CardName + " as target");
            ServerEffect.AddTarget(card);
            EffectController.ServerNotifier.AcceptTarget();
            card.Controller.deckCtrl.Shuffle();
            ServerEffect.ResolveNextSubeffect();
            return true;
        }

        EffectController.ServerNotifier.GetDeckTarget(ThisCard, this);
        return false;
    }
}
