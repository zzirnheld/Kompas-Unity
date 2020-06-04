using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapSubeffect : ServerSubeffect
{
    public int SecondTargetIndex = -2;
    public Card SecondTarget => GetTarget(SecondTargetIndex);

    public override void Resolve()
    {
        ServerGame.MoveOnBoard(Target, SecondTarget.BoardX, SecondTarget.BoardY, false, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
