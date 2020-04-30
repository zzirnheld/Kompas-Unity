using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves cards between discard/field/etc
/// </summary>
public abstract class CardChangeStateSubeffect : ServerSubeffect
{
    public int SpaceIndex = -1;

    public int X
    {
        get
        {
            if (SpaceIndex < 0) return ServerEffect.coords[ServerEffect.coords.Count + SpaceIndex].x;
            else return ServerEffect.coords[SpaceIndex].x;
        }
    }

    public int Y
    {
        get
        {
            if (SpaceIndex < 0) return ServerEffect.coords[ServerEffect.coords.Count + SpaceIndex].y;
            else return ServerEffect.coords[SpaceIndex].y;
        }
    }
}
