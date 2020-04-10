using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        Effect.targets.Add(ThisCard);
        Effect.ResolveNextSubeffect();
    }
}
