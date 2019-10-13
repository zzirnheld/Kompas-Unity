using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        parent.targets.Add(parent.thisCard);
        parent.ResolveNextSubeffect();
    }
}
