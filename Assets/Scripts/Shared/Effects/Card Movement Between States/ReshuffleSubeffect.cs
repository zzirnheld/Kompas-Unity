using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReshuffleSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        parent.serverGame.Reshuffle(Target);
        parent.ResolveNextSubeffect();
    }
}
