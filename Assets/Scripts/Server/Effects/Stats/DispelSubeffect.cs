using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispelSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        Target.Dispel(Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
