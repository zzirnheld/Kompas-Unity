using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTimesSubeffect : LoopSubeffect
{
    private int count = 0;

    protected override bool ShouldContinueLoop()
    {
        count++;
        return count < ServerEffect.X;
    }

    protected override void OnLoopExit()
    {
        count = 0;
    }
}
