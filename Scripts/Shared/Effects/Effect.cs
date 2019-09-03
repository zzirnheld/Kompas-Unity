using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects will only be resolved on server. Clients will just get to know what effects they can use
/// </summary>
public class Effect
{
    //if this effect is resolving on a server, this is the server game it's resolving on
    public ServerGame serverGame;

    //card that this is the effect of. to be set at initialization
    public Card thisCard;

    //who owns the effect. TODO set when a player activates an effect
    public int effectControllerIndex;
    public Player EffectController { get { return serverGame.Players[effectControllerIndex]; } }

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

    private int timesUsedThisTurn;
    /// <summary>
    /// The number of times this effect has been used this turn
    /// </summary>
    public int TimesUsedThisTurn { get => timesUsedThisTurn; }

    private int maxTimesCanUsePerTurn;
    /// <summary>
    /// The maximum number of times this effect can be used in a turn
    /// </summary>
    public int MaxTimesCanUsePerTurn { get => maxTimesCanUsePerTurn; }

    //get the currently resolving subeffect
    public Subeffect CurrSubeffect { get { return subeffects[effectIndex]; } }

    public Effect(SerializableEffect se, Card thisCard, int controller)
    {
        this.thisCard = thisCard;
        subeffects = new Subeffect[se.subeffects.Length];
        targets = new List<Card>();

        for (int i = 0; i < se.subeffectTypes.Length; i++)
        {
            subeffects[i] = Subeffect.FromJson(se.subeffectTypes[i], se.subeffects[i], this);
        }

        maxTimesCanUsePerTurn = se.maxTimesCanUsePerTurn;
        timesUsedThisTurn = 0;
    }

    public void ResetForTurn()
    {
        timesUsedThisTurn = 0;
    }

    public bool CanUse()
    {
        return timesUsedThisTurn < maxTimesCanUsePerTurn;
    }

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
    private void FinishResolution()
    {
        doneResolving = true;
        effectIndex = 0;
        X = 0;
        targets.Clear();
        serverGame.FinishStackEntryResolution();
    }

    //could eventually be renamed, because this same logic could be used for other things that become impossible, while a loop could be going
    public void EffectImpossible()
    {
        if (loopSubeffect == null) FinishResolution();
        else loopSubeffect.ExitLoop();
    }

    public void DeclineAnotherTarget()
    {
        if (loopSubeffect != null) loopSubeffect.ExitLoop();
    }
}
