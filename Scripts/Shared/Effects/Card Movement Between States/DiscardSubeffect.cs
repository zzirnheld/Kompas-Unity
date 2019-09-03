using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Discard();
        ServerGame?.serverNetworkCtrl.NotifyDiscard(ServerGame, Target, Target.Controller.ConnectionID);
        parent.ResolveNextSubeffect();
    }
}
