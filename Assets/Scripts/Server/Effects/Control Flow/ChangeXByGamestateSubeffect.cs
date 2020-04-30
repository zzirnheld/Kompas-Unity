using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeXByGamestateSubeffect : XByGamestateSubeffect
{
    public override void Resolve()
    {
        ServerEffect.X += Count;
        ServerEffect.ResolveNextSubeffect();
    }
}
