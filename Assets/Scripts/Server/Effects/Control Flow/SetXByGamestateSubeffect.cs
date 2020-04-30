using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXByGamestateSubeffect : XByGamestateSubeffect
{
    public override void Resolve()
    {
        Effect.X = Count;
        Effect.ResolveNextSubeffect();
    }
}
