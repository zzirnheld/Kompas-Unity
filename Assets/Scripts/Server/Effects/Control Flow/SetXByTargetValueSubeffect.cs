using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXByTargetValueSubeffect : ChangeXByTargetValueSubeffect
{
    public override void Resolve()
    {
        Effect.X = Count;
        ServerEffect.ResolveNextSubeffect();
    }
}
