using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSubeffect : Subeffect
{
    public int Player = -1;
    public int WhoDraws { get { return Player == -1 ? Effect.effectControllerIndex : Player; } }

    public override void Resolve()
    {
        var drawn = ServerGame.Draw(WhoDraws);
        if (drawn == null) Effect.EffectImpossible();
        else Effect.ResolveNextSubeffect();
    }
}
