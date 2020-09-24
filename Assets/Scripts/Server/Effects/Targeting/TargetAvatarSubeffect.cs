using KompasServer.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAvatarSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        Effect.AddTarget(Controller.Avatar);
        return ServerEffect.ResolveNextSubeffect();
    }
}
