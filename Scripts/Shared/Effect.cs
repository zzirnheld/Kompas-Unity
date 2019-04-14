using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Effect
{
    //card that this is the effect of. to be set at initialization
    [System.NonSerialized] public Card thisCard;

    //who owns the effect. TODO set when a player activates an effect
    public int effectController;

    //current subeffect that's resolving
    public int effectIndex;

    //checked by effect resolution to see if it should start resolving the next effect on the stack
    public bool doneResolving = false;

    private Subeffect[] subeffects;

    [System.NonSerialized] public List<Card> targets;

    /*
     * Effects will only be resolved on server. clients will just get to know what effects they can use
     */ 

    public void SetSubeffectsParents()
    {
        for(int i = 0; i < subeffects.Length; i++)
        {
            subeffects[i].parent = this;
        }
    }

    public void StartResolution()
    {
        ResolveSubeffect(0);
    }

    public void ResolveSubeffect(int index)
    {
        if(index >= subeffects.Length)
        {
            Finish();
            return;
        }
        effectIndex = index;
        subeffects[index].Resolve();
    }

    /// <summary>
    /// If the effect finishes resolving, this method is called.
    /// Any function can also call this effect to finish resolution early.
    /// </summary>
    public void Finish()
    {
        doneResolving = true;
        effectIndex = 0;
    }
}
