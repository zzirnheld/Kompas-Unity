using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTimesSubeffect : Subeffect
{
    public int JumpTo;

    private int count = 0;

    public override void Resolve()
    {
        if(count < parent.X)
        {
            count++;
            parent.ResolveSubeffect(JumpTo);
        }
        else
        {
            parent.ResolveNextSubeffect();
        }
    }
}
