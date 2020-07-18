using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSubeffect : CardTargetSubeffect
{
    public override bool Resolve()
    {
        ServerEffect.AddTarget(ThisCard);
        return ServerEffect.ResolveNextSubeffect();
    }
}
