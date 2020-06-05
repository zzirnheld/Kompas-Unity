using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddRestSubeffect : CardTargetSubeffect
{
    public override void Resolve()
    {
        Effect.Rest.AddRange(ServerGame.Cards.Where(c => cardRestriction.Evaluate(c)));
        ServerEffect.ResolveNextSubeffect();
    }
}
