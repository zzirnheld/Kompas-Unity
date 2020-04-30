using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawXSubeffect : Subeffect
{
    public int Player = -1;
    public int WhoDraws { get { return Player == -1 ? Effect.effectControllerIndex : Player; } }

    public override void Resolve()
    {
        var drawn = ServerGame.DrawX(WhoDraws, Effect.X);
        if (drawn.Count < Effect.X) Effect.EffectImpossible();
        else Effect.ResolveNextSubeffect();
    }
}
