using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSubeffect : Subeffect
{
    public int IndexToJumpTo;

    public override void Resolve()
    {
        //this will always jump to the given subeffect index
        parent.ResolveSubeffect(IndexToJumpTo);
    }
}
