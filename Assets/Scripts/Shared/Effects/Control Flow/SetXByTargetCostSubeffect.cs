using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXByTargetCostSubeffect : Subeffect
{
    public override void Resolve()
    {
        Effect.X = Target.Cost;
        Effect.ResolveNextSubeffect();
    }
}
