using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalEndSubeffect : Subeffect
{
    public const string XLessThan0 = "X<0";
    public const string XLessThanEqual0 = "X<=0";

    public string Condition;

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
            default:
                throw new System.ArgumentException($"Condition {Condition} invalid for conditional end subeffect");
        }

        if (end) Effect.EffectImpossible();
        else Effect.ResolveNextSubeffect();
    }
}
