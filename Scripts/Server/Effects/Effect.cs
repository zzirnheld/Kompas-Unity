using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    //if this effect is resolving on a server, this is the server game it's resolving on
    public ServerGame serverGame;

    //card that this is the effect of. to be set at initialization
    public Card thisCard;

    //who owns the effect. TODO set when a player activates an effect
    public int effectController;

    //current subeffect that's resolving
    public int effectIndex;

    //checked by effect resolution to see if it should start resolving the next effect on the stack
    public bool doneResolving = false;

    public CountXLoopSubeffect loopSubeffect = null;

    private Subeffect[] subeffects;
    public Subeffect[] Subeffects { get => subeffects; }

    public List<Card> targets;
    public List<Vector2Int> coords;

    /// <summary>
    /// X value as listed on cards
    /// </summary>
    public int X = 0;
    
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
                case SerializableEffect.SubeffectType.DeckTarget:
                    DeckTargetSubeffect deckTarget = JsonUtility.FromJson<DeckTargetSubeffect>(se.subeffects[i]);
                    deckTarget.cardRestriction.subeffect = deckTarget;
                    subeffects[i] = deckTarget;
                    break;
                case SerializableEffect.SubeffectType.DiscardTarget:
                    DiscardTargetSubeffect discardTarget = JsonUtility.FromJson<DiscardTargetSubeffect>(se.subeffects[i]);
                    discardTarget.cardRestriction.subeffect = discardTarget;
                    subeffects[i] = discardTarget;
                    break;
                case SerializableEffect.SubeffectType.HandTarget:
                    HandTargetSubeffect handTarget = JsonUtility.FromJson<HandTargetSubeffect>(se.subeffects[i]);
                    handTarget.cardRestriction.subeffect = handTarget;
                    subeffects[i] = handTarget;
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
        X = 0;
        targets.Clear();
        (thisCard.game as ServerGame).FinishStackEntryResolution();
    }

    //could eventually be renamed, because this same logic could be used for other things that become impossible, while a loop could be going
    public void NoTargetExists()
    {
        if (loopSubeffect == null) FinishResolution();
        else loopSubeffect.ExitLoop();
    }

    public void DeclineAnotherTarget()
    {
        if (loopSubeffect != null) loopSubeffect.ExitLoop();
    }
}
