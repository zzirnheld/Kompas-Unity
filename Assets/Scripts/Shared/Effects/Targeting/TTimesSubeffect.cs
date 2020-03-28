using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTimesSubeffect : LoopSubeffect
{
    public int T;
    private int count = 0;

    protected override void OnLoopExit()
    {
        count = 0;
    }

    protected override bool ShouldContinueLoop()
    {
        count++;
        return count < T;
    }
}
