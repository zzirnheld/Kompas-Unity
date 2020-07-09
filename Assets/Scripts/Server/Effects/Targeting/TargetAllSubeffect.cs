using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TargetAllSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        var targets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));
        //check what targets there are now, before you add them, to not mess with NotAlreadyTarget restriction
        //because Linq executes lazily, it would otherwise add the targets, then re-execute the query and not find any
        bool any = targets.Any();
        Effect.AddTargetRange(targets);        

        if (any) ServerEffect.ResolveNextSubeffect();
        else ServerEffect.EffectImpossible();
    }
}

