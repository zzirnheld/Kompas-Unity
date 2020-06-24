using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Move(X, Y, false, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
