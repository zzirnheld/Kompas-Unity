using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpendMovementSubeffect : ServerSubeffect
{
    public int Multiplier = 0;
    public int Modifier = 0;
    public int Divisor = 1;

    public override bool Resolve()
    {
        var spaces = Effect.X * Multiplier / Divisor + Modifier;
        if (Target.SpacesCanMove >= spaces)
        {
            Target.SpacesMoved += spaces;
            return ServerEffect.ResolveNextSubeffect();
        }
        else return ServerEffect.EffectImpossible();
    }
}
