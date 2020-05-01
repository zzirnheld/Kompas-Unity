using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawXSubeffect : ServerSubeffect
{
    public int Player = -1;
    public int WhoDraws { get { return Player == -1 ? ServerEffect.ServerController.index : Player; } }

    public override void Resolve()
    {
        var drawn = ServerGame.DrawX(WhoDraws, ServerEffect.X);
        if (drawn.Count < ServerEffect.X) ServerEffect.EffectImpossible();
        else ServerEffect.ResolveNextSubeffect();
    }
}
