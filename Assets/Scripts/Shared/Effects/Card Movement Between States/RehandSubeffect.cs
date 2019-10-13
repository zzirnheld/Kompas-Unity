using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Rehand(Target.OwnerIndex);
        ServerGame?.serverNotifier.NotifyRehand(Target);
        parent.ResolveNextSubeffect();
    }
}
