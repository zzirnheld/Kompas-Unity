using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPipsSubeffect : Subeffect
{
    public int xMultiplier;
    public int xDivisor;
    public int modifier;
    public int playerOffset; //0 for controller, 1 for controller's enemy

    public override void Resolve()
    {
        //could also be                           playerOffest == parent.effectController ? 0 : 1
        Player player = parent.serverGame.Players[playerOffset + parent.effectControllerIndex % 2];
        player.pips += (xMultiplier * parent.X / xDivisor) + modifier;
        ServerGame.serverNetworkCtrl.NotifySetPips(ServerGame, parent.effectControllerIndex, player.pips, parent.thisCard.Controller.ConnectionID);
    }
}
