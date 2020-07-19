using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSubeffect : ServerSubeffect
{
    public int IndexToJumpTo;

    public override bool Resolve()
    {
        //this will always jump to the given subeffect index
        return ServerEffect.ResolveSubeffect(IndexToJumpTo);
    }
}
