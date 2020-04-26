using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionalEndSubeffect : Subeffect
{
    public const string XLessThan0 = "X<0";
    public const string XLessThanEqual0 = "X<=0";
    public const string XGreaterThanConst = "X>C";
    public const string XLessThanConst = "X<C";
    public const string NoneFitRestriction = "None Fit Restriction";

    public int C = 0;
    public CardRestriction CardRestriction = new CardRestriction();

    public string Condition;

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        CardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        bool end;
        switch (Condition)
        {
            case XLessThan0:
                end = Effect.X < 0;
                break;
            case XLessThanEqual0:
                end = Effect.X <= 0;
                break;
            case XGreaterThanConst:
                end = Effect.X > C;
                break;
            case XLessThanConst:
                end = Effect.X < C;
                break;
            case NoneFitRestriction:
                end = !ServerGame.cards.Any(c => CardRestriction.Evaluate(c.Value));
                break;
            default:
                throw new System.ArgumentException($"Condition {Condition} invalid for conditional end subeffect");
        }

        if (end) Effect.EffectImpossible();
        else Effect.ResolveNextSubeffect();
    }
}
