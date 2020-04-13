using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReshuffleSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        ServerGame.Reshuffle(Target, Effect);
        Effect.ResolveNextSubeffect();
    }
}
