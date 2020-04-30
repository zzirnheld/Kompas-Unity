using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAugmentedCardSubeffect : Subeffect
{
    public override void Resolve()
    {
        if(Effect.thisCard is AugmentCard aug)
        {
            Effect.targets.Add(aug.AugmentedCard);
        }
        Effect.ResolveNextSubeffect();
    }
}
