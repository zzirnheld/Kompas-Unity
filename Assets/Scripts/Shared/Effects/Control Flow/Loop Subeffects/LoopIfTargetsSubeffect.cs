using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoopIfTargetsSubeffect : LoopSubeffect
{
    protected override void OnLoopExit() { }

    protected override bool ShouldContinueLoop()
    {
        return parent.targets.Any();
    }
}
