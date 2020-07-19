using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispelSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        Target.Dispel(Effect);
        return ServerEffect.ResolveNextSubeffect();
    }
}
