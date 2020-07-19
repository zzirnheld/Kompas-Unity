using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXByTargetValueSubeffect : ChangeXByTargetValueSubeffect
{
    public override bool Resolve()
    {
        Effect.X = Count;
        return ServerEffect.ResolveNextSubeffect();
    }
}
