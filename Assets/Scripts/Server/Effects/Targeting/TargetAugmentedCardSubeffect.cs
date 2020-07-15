using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAugmentedCardSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if (ServerEffect.Source.AugmentedCard == null) ServerEffect.EffectImpossible();
        else
        {
            ServerEffect.AddTarget(Source.AugmentedCard);
            ServerEffect.ResolveNextSubeffect();
        }
    }
}
