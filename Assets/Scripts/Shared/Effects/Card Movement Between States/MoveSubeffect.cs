using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        ServerGame.MoveOnBoard(Target, X, Y, Effect);
        Effect.ResolveNextSubeffect();
    }
}
