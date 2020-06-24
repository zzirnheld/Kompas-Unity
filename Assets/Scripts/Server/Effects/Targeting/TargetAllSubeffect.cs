using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TargetAllSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        var targets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));
        Effect.Targets.AddRange(targets);

        if (targets.Any()) ServerEffect.ResolveNextSubeffect();
        else ServerEffect.EffectImpossible();
    }
}

