using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCardOnBoardSubeffect : Subeffect
{
    //there will be a different target effect for board, hand, discard, deck, and combos of these


    public CardRestriction cardRestriction;

    /*
         * TODO implementation:
         * have a currently resolving effect in game
         * when reach this, IF THIS EFFECT IS OWNED BY THAT PLAYER
         * set the current effect
         * in the effect, set current restriction for target
         * set targeting mode
         * 
         * attempt to get a valid target
         * if the client selects a target they think is valid, send that to the server
         * if the server accepts that as a valid target, it sends a "continue resolution" to the client
         * otherwise, the server sends a "get another target" request, which will clear the current target?
         * 
         * IF THIS EFFECT IS OWNED BY NOT THE CURRENT PLAYER
         * wait until the server sends a valid target, then continue resolution
         */ 

    public override void Resolve()
    {
        cardRestriction.subeffect = this;

        //no matter what, check first that there exist valid targets. if there exist no valid targets, finish resolution here
        if (parent.thisCard.game.NoValidCardOnBoardTarget(cardRestriction))
        {
            parent.Finish();
            return;
        }

        //if this is a server effect, then let the server game know what restriction to look for a target for
        if(parent.thisCard.game is ServerGame)
        {

        }
        //otherwise, if this is a client and the effect is owned by the other player, do nothing
        else if(parent.effectController == 1)
        {
            //let the client game know what effect to continue resolution of
        }

        //then wait for whatever to call the continue method
    }

    
}
