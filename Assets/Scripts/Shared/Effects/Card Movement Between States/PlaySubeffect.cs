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
            if (SpaceIndex < 0) return Effect.coords[Effect.coords.Count + SpaceIndex].x;
            else return Effect.coords[SpaceIndex].x;
        }
    }

    public int Y
    {
        get
        {
            if (SpaceIndex < 0) return Effect.coords[Effect.coords.Count + SpaceIndex].y;
            else return Effect.coords[SpaceIndex].y;
        }
    }

    public override void Resolve()
    {
        Effect.serverGame.Play(Target, X, Y, EffectController);
        Effect.ResolveNextSubeffect();
    }
}
