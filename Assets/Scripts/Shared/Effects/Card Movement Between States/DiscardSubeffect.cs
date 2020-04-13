using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Debug.Log("Resolving discard subeffect");
        ServerGame.Discard(Target, Effect);
        Effect.ResolveNextSubeffect();
    }
}
