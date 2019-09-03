using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySubeffect : CardChangeStateSubeffect
{
    //same rules as CardChangeSubeffect's target index
    public int SpaceIndex;

    public int X
    {
        get
        {
            if (SpaceIndex < 0) return parent.coords[parent.coords.Count + SpaceIndex].x;
            else return parent.coords[SpaceIndex].x;
        }
    }

    public int Y
    {
        get
        {
            if (SpaceIndex < 0) return parent.coords[parent.coords.Count + SpaceIndex].y;
            else return parent.coords[SpaceIndex].y;
        }
    }

    public override void Resolve()
    {
        Target.Play(X, Y, parent.effectControllerIndex);
        ServerGame?.serverNetworkCtrl.NotifyPlay(ServerGame, Target, X, Y, Target.Controller.ConnectionID);
        parent.ResolveNextSubeffect();
    }
}
