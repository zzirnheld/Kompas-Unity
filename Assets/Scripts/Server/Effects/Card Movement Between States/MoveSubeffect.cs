using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        var (x, y) = Space;
        if (Target.Move(x, y, false, Effect)) ServerEffect.ResolveNextSubeffect();
        else ServerEffect.EffectImpossible();
    }
}
