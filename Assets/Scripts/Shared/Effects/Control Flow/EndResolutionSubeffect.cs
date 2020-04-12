using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndResolutionSubeffect : Subeffect
{
    public override void Resolve()
    {
        Effect.EffectImpossible();
    }
}
