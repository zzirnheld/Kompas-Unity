using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeXByGamestateSubeffect : XByGamestateSubeffect
{
    public override bool Resolve()
    {
        ServerEffect.X += Count;
        return ServerEffect.ResolveNextSubeffect();
    }
}
