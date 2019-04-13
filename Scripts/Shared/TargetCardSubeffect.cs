using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetCardSubeffect : Subeffect
{
    public CardRestriction cardRestriction;

    public TargetCardSubeffect(Effect parent) : base(parent)
    {
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
    }
}
