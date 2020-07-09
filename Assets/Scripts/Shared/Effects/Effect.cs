using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effects will only be resolved on server. Clients will just get to know what effects they can use
/// </summary>
public abstract class Effect : IStackable
{
    public Game Game => Source.Game;

    public GameCard Source { get; }
    public abstract Player Controller { get; set; }

    //subeffects
    public abstract Subeffect[] Subeffects { get; }
    //current subeffect that's resolving
    public int SubeffectIndex { get; protected set; }
    public Subeffect CurrSubeffect => Subeffects[SubeffectIndex];

    public readonly int EffectIndex;

    //Targets
    public List<GameCard> Targets { get; } = new List<GameCard>();
    public List<(int x, int y)> Coords { get; private set; } = new List<(int x, int y)>();
    public List<GameCard> Rest { get; private set; } = new List<GameCard>();

    //Trigger
    public abstract Trigger Trigger { get; }
    public ActivationRestriction ActivationRestriction { get; }
    public string Blurb { get; }
    public ActivationContext CurrActivationContext { get; protected set; }

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

    public Effect(ActivationRestriction restriction, GameCard source, string blurb, int effIndex)
    {
        Source = source ?? throw new System.ArgumentNullException($"Effect cannot be attached to null card");
        ActivationRestriction = restriction;
        ActivationRestriction.Initialize(this);
        Blurb = blurb;
        EffectIndex = effIndex;
        TimesUsedThisTurn = 0;
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

    public virtual void AddTarget(GameCard card)
    {
        Targets.Add(card);
    }

    public GameCard TargetAt(int index)
    {
        return Targets.ElementAt(index);
    }
}
