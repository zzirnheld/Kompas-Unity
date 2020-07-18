using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndResolutionSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        return ServerEffect.ResolveSubeffect(ServerEffect.Subeffects.Length);
    }
}
