using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSubeffect : ServerSubeffect
{
    public int SecondTargetIndex = -2;
    public GameCard SecondTarget => GetTarget(SecondTargetIndex);

    public override bool Resolve()
    {
        if (Target != null && SecondTarget != null && Target.Move(SecondTarget.BoardX, SecondTarget.BoardY, false, ServerEffect))
            return ServerEffect.ResolveNextSubeffect();
        else return ServerEffect.EffectImpossible();
    }
}
