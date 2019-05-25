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

    private Subeffect[] subeffects;
    public Subeffect[] Subeffects { get => subeffects; }

    public List<Card> targets;
    
    //get the currently resolving subeffect
    public Subeffect CurrSubeffect { get { return subeffects[effectIndex]; } }

    public Effect(SerializableEffect se, Card thisCard, int controller)
    {
        this.thisCard = thisCard;
        subeffects = new Subeffect[se.subeffects.Length];
        targets = new List<Card>();

        for (int i = 0; i < se.subeffectTypes.Length; i++)
        {
            switch (se.subeffectTypes[i])
            {
                case SerializableEffect.SubeffectType.TargetCardOnBoard:
                    BoardTargetSubeffect tcob = JsonUtility.FromJson<BoardTargetSubeffect>(se.subeffects[i]);
                    tcob.cardRestriction.subeffect = tcob;
                    subeffects[i] = tcob;
                    break;
                case SerializableEffect.SubeffectType.ChangeNESW:
                    subeffects[i] = JsonUtility.FromJson<ChangeNESWSubeffect>(se.subeffects[i]);
                    break;
                default:
                    Debug.Log("Unrecognized effect type enum for loading effect in effect constructor");
                    subeffects[i] = null;
                    break;
            }

            if (subeffects[i] != null) subeffects[i].parent = this;
        }
    }

    /*
     * Effects will only be resolved on server. clients will just get to know what effects they can use
     */

    public void StartResolution()
    {
        thisCard.game.CurrEffect = this;
        ResolveSubeffect(0);
    }

    public void ResolveNextSubeffect()
    {
        ResolveSubeffect(effectIndex + 1);
    }

    public void ResolveSubeffect(int index)
    {
        if(index >= subeffects.Length)
        {
            FinishResolution();
            return;
        }
        effectIndex = index;
        subeffects[index].Resolve();
    }

    /// <summary>
    /// If the effect finishes resolving, this method is called.
    /// Any function can also call this effect to finish resolution early.
    /// </summary>
    public void FinishResolution()
    {
        doneResolving = true;
        effectIndex = 0;
        (thisCard.game as ServerGame).FinishStackEntryResolution();
    }
}
