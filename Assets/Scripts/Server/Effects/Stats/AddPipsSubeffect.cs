using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPipsSubeffect : ServerSubeffect
{
    public int xMultiplier = 0;
    public int xDivisor = 1;
    public int modifier = 0;
    public int playerOffset = 0; //0 for controller, 1 for controller's enemy

    public override void Resolve()
    {
        //could also be                           playerOffest == parent.effectController ? 0 : 1
        Player player = ServerEffect.serverGame.Players[playerOffset + ServerEffect.ServerController.index % 2];
        player.pips += (xMultiplier * ServerEffect.X / xDivisor) + modifier;
        EffectController.ServerNotifier.NotifySetPips(player.pips);
        ServerEffect.ResolveNextSubeffect();
    }
}
