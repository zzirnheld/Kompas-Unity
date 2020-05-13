using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAugmentedCardSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if(ServerEffect.thisCard is AugmentCard aug)
        {
            ServerEffect.Targets.Add(aug.AugmentedCard);
        }
        ServerEffect.ResolveNextSubeffect();
    }
}
