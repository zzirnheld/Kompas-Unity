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
            ServerEffect.Targets.Add(Source.AugmentedCard);
            ServerEffect.ResolveNextSubeffect();
        }
    }
}
