using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAugmentedCardSubeffect : ServerSubeffect
{
    public override bool Resolve()
    {
        if (ServerEffect.Source.AugmentedCard == null) return ServerEffect.EffectImpossible();
        else
        {
            ServerEffect.AddTarget(Source.AugmentedCard);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}
