using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoopWhileHaveTargetsSubeffect : LoopSubeffect
{
    protected override bool ShouldContinueLoop => ServerEffect.Targets.Any();
}
