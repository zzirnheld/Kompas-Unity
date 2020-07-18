using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXTargetSSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        ServerEffect.X = Target.S;
        return ServerEffect.ResolveNextSubeffect();
    }
}
