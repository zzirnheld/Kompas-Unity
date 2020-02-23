using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        parent.serverGame.Rehand(Target);
        parent.ResolveNextSubeffect();
    }
}
