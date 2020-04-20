using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegateSubeffect : Subeffect
{
    public override void Resolve()
    {
        ServerGame.Negate(Target);
        Effect.ResolveNextSubeffect();
    }
}
