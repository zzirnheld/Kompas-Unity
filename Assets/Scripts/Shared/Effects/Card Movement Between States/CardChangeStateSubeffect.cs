using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves cards between discard/field/etc
/// </summary>
public abstract class CardChangeStateSubeffect : Subeffect
{
    public int SpaceIndex = -1;

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
}
