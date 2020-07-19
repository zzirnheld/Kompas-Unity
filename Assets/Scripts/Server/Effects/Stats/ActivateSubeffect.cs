using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        Target.SetActivated(true, Effect);
        return ServerEffect.ResolveNextSubeffect();
    }
}
