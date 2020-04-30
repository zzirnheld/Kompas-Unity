using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        ServerEffect.targets.Add(ThisCard);
        ServerEffect.ResolveNextSubeffect();
    }
}
