using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects will only be resolved on server. Clients will just get to know what effects they can use
/// </summary>
public abstract class Effect
{
    public Game Game => Source.game;

    //card that this is the effect of. to be set at initialization
    public Card thisCard;
    public Card Source { get { return thisCard; } }
    public abstract Player Controller { get; set; }

    //subeffects
    public abstract Subeffect[] Subeffects { get; }
    //current subeffect that's resolving
    public int SubeffectIndex { get; protected set; }
    public Subeffect CurrSubeffect => Subeffects[SubeffectIndex];

    public int EffectIndex => System.Array.IndexOf(thisCard.Effects, this);

    public List<Card> Targets { get; private set; }
    public List<Vector2Int> Coords { get; private set; }
    public List<Card> Rest { get; private set; }

    public abstract Trigger Trigger { get; }

    private int negations = 0;
    public bool Negated {
        get => negations > 0;
        set
        {
            if (value) negations++;
            else negations--;
        }
    }

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
        Targets = new List<Card>();
        Rest = new List<Card>();
        Coords = new List<Vector2Int>();
    }

    public void ResetForTurn()
    {
        TimesUsedThisTurn = 0;
    }

    public virtual void Negate()
    {
        Negated = true;
    }
}
