using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAllSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        bool found = false;
        foreach (KeyValuePair<int, GameCard> pair in ServerGame.cardsByID)
        {
            if (cardRestriction.Evaluate(pair.Value))
            {
                ServerEffect.Targets.Add(pair.Value);
                found = true;
            }
        }

        if (found) ServerEffect.ResolveNextSubeffect();
        else ServerEffect.EffectImpossible();
    }
}

