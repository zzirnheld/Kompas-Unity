using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target.Location == CardLocation.Field)
        {
            var (x, y) = Space;
            Target.Move(x, y, false, Effect);
            ServerEffect.ResolveNextSubeffect();
        }
        else ServerEffect.EffectImpossible();
    }
}
