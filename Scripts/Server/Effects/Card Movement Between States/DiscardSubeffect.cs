using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        parent.serverGame.Discard(GetTarget(), parent.effectController);
        parent.ResolveNextSubeffect();
    }
}
