using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSubeffect : ServerSubeffect
{
    public int Player = -1;
    public int WhoDraws { get { return Player == -1 ? ServerEffect.effectControllerIndex : Player; } }

    public override void Resolve()
    {
        var drawn = ServerGame.Draw(WhoDraws);
        if (drawn == null) ServerEffect.EffectImpossible();
        else ServerEffect.ResolveNextSubeffect();
    }
}
