using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXByGamestateSubeffect : XByGamestateSubeffect
{
    public override bool Resolve()
    {
        ServerEffect.X = Count;
        return ServerEffect.ResolveNextSubeffect();
    }
}
