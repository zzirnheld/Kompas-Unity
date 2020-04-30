using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects will only be resolved on server. Clients will just get to know what effects they can use
/// </summary>
public abstract class Effect
{
    //card that this is the effect of. to be set at initialization
    public Card thisCard;
    public Card Source { get { return thisCard; } }

    //who owns the effect. TODO set when a player activates an effect
    public int effectControllerIndex;
    public abstract Player Controller { get; }

    //current subeffect that's resolving
    public int subeffectIndex;

    public int EffectIndex { get { return System.Array.IndexOf(thisCard.Effects, this); } }

    //checked by effect resolution to see if it should start resolving the next effect on the stack
    public bool doneResolving = false;

    public ServerSubeffect OnImpossible = null;

    public List<Card> targets;
    public List<Vector2Int> coords;

    public bool Negated { get; protected set; }

    /// <summary>
    /// X value as listed on cards
    /// </summary>
    public int X = 0;
    /// <summary>
    /// The number of times this effect has been used this turn
    /// </summary>
    public int TimesUsedThisTurn { get; protected set; }
    /// <summary>
    /// The maximum number of times this effect can be used in a turn
    /// </summary>
    public int? MaxTimesCanUsePerTurn { get; }


    public Effect(int? maxPerTurn)
    {
        this.MaxTimesCanUsePerTurn = maxPerTurn;
        TimesUsedThisTurn = 0;
    }


    public void ResetForTurn()
    {
        TimesUsedThisTurn = 0;
    }

    public bool CanUse()
    {
        return TimesUsedThisTurn < MaxTimesCanUsePerTurn;
    }

    public virtual void Negate()
    {
        Negated = true;
    }
}
