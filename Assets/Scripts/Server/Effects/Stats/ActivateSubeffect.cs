using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        ServerGame.SetActivated(Target, true, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
