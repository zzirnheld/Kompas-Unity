using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegateSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        Target.SetNegated(true, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
