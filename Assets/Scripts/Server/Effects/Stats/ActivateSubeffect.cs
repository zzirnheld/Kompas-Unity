using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        Target.SetActivated(true, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
