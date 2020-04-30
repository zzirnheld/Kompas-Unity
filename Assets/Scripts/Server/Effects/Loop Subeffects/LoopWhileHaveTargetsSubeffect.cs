using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoopWhileHaveTargetsSubeffect : LoopSubeffect
{
    protected override void OnLoopExit() { }

    protected override bool ShouldContinueLoop()
    {
        return ServerEffect.targets.Any();
    }
}
