using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        ServerEffect.Targets.Add(ThisCard);
        ServerEffect.ResolveNextSubeffect();
    }
}
