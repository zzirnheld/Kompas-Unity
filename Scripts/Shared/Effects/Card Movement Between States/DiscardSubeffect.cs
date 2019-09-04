using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Debug.Log("Resolving discard subeffect");
        ServerGame?.serverNetworkCtrl.NotifyDiscard(ServerGame, Target, Target.Controller.ConnectionID);
        Target.Discard();
        parent.ResolveNextSubeffect();
    }
}
