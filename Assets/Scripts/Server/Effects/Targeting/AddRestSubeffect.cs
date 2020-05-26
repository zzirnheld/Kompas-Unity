using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddRestSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        var cards = ServerGame.cards.Values;
        Effect.Rest.AddRange(cards.Where(c => cardRestriction.Evaluate(c)));

        ServerEffect.ResolveNextSubeffect();
    }
}
