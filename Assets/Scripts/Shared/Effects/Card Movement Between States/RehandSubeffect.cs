using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Effect.serverGame.Rehand(Target);
        Effect.ResolveNextSubeffect();
    }
}
