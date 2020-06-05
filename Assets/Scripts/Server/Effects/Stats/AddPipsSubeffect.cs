using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPipsSubeffect : ServerSubeffect
{
    public int xMultiplier = 0;
    public int xDivisor = 1;
    public int modifier = 0;
    public int playerOffset = 0; //0 for controller, 1 for controller's enemy

    //could also be                                         playerOffest == parent.effectController ? 0 : 1
    private Player Player => ServerEffect.serverGame.Players[playerOffset + ServerEffect.ServerController.index % 2];
    private int Count => (xMultiplier * ServerEffect.X / xDivisor) + modifier;

    public override void Resolve()
    {
        Player.pips += Count;
        EffectController.ServerNotifier.NotifySetPips(Player.pips);
        ServerEffect.ResolveNextSubeffect();
    }
}
