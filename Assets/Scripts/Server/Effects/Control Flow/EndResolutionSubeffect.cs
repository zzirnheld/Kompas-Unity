using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndResolutionSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        ServerEffect.EffectImpossible();
    }
}
