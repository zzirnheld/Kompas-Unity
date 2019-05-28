using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reshuffle : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        parent.serverGame.Reshuffle(GetTarget(), parent.effectController);
        parent.ResolveNextSubeffect();
    }
}
