using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawXSubeffect : ServerSubeffect
{
    public int Player = -1;
    public int WhoDraws { get { return Player == -1 ? ServerEffect.ServerController.index : Player; } }
    public int XMultiplier = 1;
    public int XDivisor = 1;
    public int Modifier = 0;
    public int Count => (ServerEffect.X * XMultiplier / XDivisor) + Modifier;

    public override void Resolve()
    {
        var drawn = ServerGame.DrawX(WhoDraws, Count);
        if (drawn.Count < Count) ServerEffect.EffectImpossible();
        else ServerEffect.ResolveNextSubeffect();
    }
}
