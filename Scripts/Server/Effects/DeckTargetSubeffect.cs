using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckTargetSubeffect : CardTargetSubeffect
{

    public override void Resolve()
    {
        cardRestriction.subeffect = this;

        //check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (!parent.thisCard.game.ExistsDeckTarget(cardRestriction, parent.thisCard.Owner))
        {
            Debug.Log("No target exists for " + parent.thisCard.CardName + " effect");
            parent.FinishResolution();
            return;
        }

        //ask the client that is this effect's controller for a target. 
        //give the card if whose effect it is, the index of the effect, and the index of the subeffect
        //since only the server resolves effects, this should never be called for a client. 
        (parent.thisCard.game.networkCtrl as ServerNetworkController).GetDeckTarget(
                                            parent.thisCard.game as ServerGame,
                                            parent.effectController,
                                            parent.thisCard,
                                            System.Array.IndexOf(parent.thisCard.Effects, this),
                                            parent.effectIndex);

        //then wait for the network controller to call the continue method
    }
}
