using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpellCSubeffect : ServerSubeffect
{
    public int XModifier = 0;
    public int XMultiplier = 0;
    public int XDivisor = 1;

    public override void Resolve()
    {
        Target.SetC(ServerEffect.X * XMultiplier / XDivisor + XModifier, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
