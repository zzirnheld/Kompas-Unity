using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAllSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        foreach (KeyValuePair<int, Card> pair in ServerGame.cards)
        {
            if (cardRestriction.Evaluate(pair.Value))
            {
                parent.targets.Add(pair.Value);
            }
        }

        parent.ResolveNextSubeffect();
    }
}
