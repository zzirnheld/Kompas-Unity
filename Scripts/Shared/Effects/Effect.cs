using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects will only be resolved on server. Clients will just get to know what effects they can use
/// </summary>
public class Effect : IStackable
{
    //if this effect is resolving on a server, this is the server game it's resolving on
    public ServerGame serverGame;

    //card that this is the effect of. to be set at initialization
    public Card thisCard;

    //who owns the effect. TODO set when a player activates an effect
    public int effectControllerIndex;
    public Player EffectController { get { return serverGame.Players[effectControllerIndex]; } }

    //current subeffect that's resolving
    public int subeffectIndex;

    public int EffectIndex { get { return System.Array.IndexOf(thisCard.Effects, this); } }

    //checked by effect resolution to see if it should start resolving the next effect on the stack
    public bool doneResolving = false;

    public CountXLoopSubeffect loopSubeffect = null;

    private Subeffect[] subeffects;
    public Subeffect[] Subeffects { get => subeffects; }

    private Trigger trigger;
    public Trigger Trigger { get => trigger; }

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

    private int? maxTimesCanUsePerTurn;
    /// <summary>
    /// The maximum number of times this effect can be used in a turn
    /// </summary>
    public int? MaxTimesCanUsePerTurn { get => maxTimesCanUsePerTurn; }

    //get the currently resolving subeffect
    public Subeffect CurrSubeffect { get { return subeffects[subeffectIndex]; } }

    public Effect(SerializableEffect se, Card thisCard, int controller)
    {
        this.thisCard = thisCard;
        serverGame = thisCard.Controller.serverGame;
        subeffects = new Subeffect[se.subeffects.Length];
        targets = new List<Card>();
        coords = new List<Vector2Int>();

        if (!string.IsNullOrEmpty(se.trigger))
        {
            trigger = Trigger.FromJson(se.triggerCondition, se.trigger, this);
        }

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

    public void PushToStack(bool checkForResponses)
    {
        serverGame.PushToStack(this, effectControllerIndex, checkForResponses);
    }

    public void StartResolution()
    {
        thisCard.game.CurrEffect = this;
        ResolveSubeffect(0);
    }

    public void ResolveNextSubeffect()
    {
        ResolveSubeffect(subeffectIndex + 1);
    }

    public void ResolveSubeffect(int index)
    {
        if(index >= subeffects.Length)
        {
            FinishResolution();
            return;
        }
        subeffectIndex = index;
        subeffects[index].Resolve();
    }

    /// <summary>
    /// If the effect finishes resolving, this method is called.
    /// Any function can also call this effect to finish resolution early.
    /// </summary>
    private void FinishResolution()
    {
        doneResolving = true;
        subeffectIndex = 0;
        X = 0;
        targets.Clear();
        serverGame.serverNotifier.AcceptTarget(EffectController.ConnectionID);
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
