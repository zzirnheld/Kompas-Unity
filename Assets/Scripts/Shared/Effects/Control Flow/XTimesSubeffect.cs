using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTimesSubeffect : Subeffect
{
    public int JumpTo;

    private int count = 0;

    public override void Resolve()
    {
        count++;
        if (count < parent.X)
        {
            parent.ResolveSubeffect(JumpTo);
        }
        else
        {
            parent.ResolveNextSubeffect();
        }
    }
}
