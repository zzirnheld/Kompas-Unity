using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXByTargetCostSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if(Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }

        ServerEffect.X = Target.Cost;
        ServerEffect.ResolveNextSubeffect();
    }
}
