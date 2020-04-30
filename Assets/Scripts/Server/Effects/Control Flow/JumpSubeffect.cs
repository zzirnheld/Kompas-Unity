using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSubeffect : ServerSubeffect
{
    public int IndexToJumpTo;

    public override void Resolve()
    {
        //this will always jump to the given subeffect index
        ServerEffect.ResolveSubeffect(IndexToJumpTo);
    }
}
