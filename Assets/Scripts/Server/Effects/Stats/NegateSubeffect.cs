using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegateSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        Target.SetNegated(true, ServerEffect);
        return ServerEffect.ResolveNextSubeffect();
    }
}
