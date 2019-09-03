using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reshuffle : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Reshuffle();
        ServerGame?.serverNetworkCtrl.NotifyReshuffle(ServerGame, Target, Target.Controller.ConnectionID);
        parent.ResolveNextSubeffect();
    }
}
