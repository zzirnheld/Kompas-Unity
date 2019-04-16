using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    //card that this is the effect of. to be set at initialization
    public Card thisCard;

    //who owns the effect. TODO set when a player activates an effect
    public int effectController;

    //current subeffect that's resolving
    public int effectIndex;

    //checked by effect resolution to see if it should start resolving the next effect on the stack
    public bool doneResolving = false;

    private List<Subeffect> subeffects;
    public List<Subeffect> Subeffects { get => subeffects; }

    public List<Card> targets;
    
    //get the currently resolving subeffect
    public Subeffect CurrentlyResolvingSubeffect { get { return subeffects[effectIndex]; } }

    public Effect(SerializableEffect se)
    {
        for (int i = 0; i < se.subeffectTypes.Length; i++)
        {
            switch (se.subeffectTypes[i])
            {
                case SerializableEffect.SubeffectType.TargetCardOnBoardSubeffect:
                    subeffects.Add(JsonUtility.FromJson<TargetCardOnBoardSubeffect>(se.subeffects[i]));
                    break;
                default:
                    Debug.Log("Unrecognized effect type enum for loading effect in effect constructor");
                    break;
            }
        }
    }

    /*
     * Effects will only be resolved on server. clients will just get to know what effects they can use
     */

    public void SetSubeffectsParents()
    {
        for(int i = 0; i < subeffects.Count; i++)
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
        if(index >= subeffects.Count)
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
