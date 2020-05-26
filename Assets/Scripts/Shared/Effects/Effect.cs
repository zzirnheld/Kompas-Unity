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
    public ActivationRestriction ActivationRestriction { get; }

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
    /// X value for card effect text (not coordinates)
    /// </summary>
    public int X = 0;
    public int TimesUsedThisTurn { get; protected set; }
    public int TimesUsedThisRound { get; protected set; }

    public Effect(ActivationRestriction restriction)
    {
        ActivationRestriction = restriction;
        ActivationRestriction.Initialize(this);
        TimesUsedThisTurn = 0;
        Targets = new List<Card>();
        Rest = new List<Card>();
        Coords = new List<Vector2Int>();
    }

    public void ResetForTurn(Player turnPlayer)
    {
        TimesUsedThisTurn = 0;
        if (turnPlayer == Source.Controller) TimesUsedThisRound = 0;
    }

    public virtual void Negate()
    {
        Negated = true;
    }
}
