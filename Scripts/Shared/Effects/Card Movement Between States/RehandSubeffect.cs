using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Rehand(Target.OwnerIndex);
        ServerGame?.serverNetworkCtrl.NotifyRehand(ServerGame, Target);
        parent.ResolveNextSubeffect();
    }
}
