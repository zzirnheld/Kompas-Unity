using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSubeffect : ServerSubeffect
{
    public int SecondTargetIndex = -2;
    public GameCard SecondTarget => GetTarget(SecondTargetIndex);

    public override void Resolve()
    {
        Target.Move(SecondTarget.BoardX, SecondTarget.BoardY, false, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
