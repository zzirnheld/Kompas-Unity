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
            if (SpaceIndex < 0) return ServerEffect.Coords[ServerEffect.Coords.Count + SpaceIndex].x;
            else return ServerEffect.Coords[SpaceIndex].x;
        }
    }

    public int Y
    {
        get
        {
            if (SpaceIndex < 0) return ServerEffect.Coords[ServerEffect.Coords.Count + SpaceIndex].y;
            else return ServerEffect.Coords[SpaceIndex].y;
        }
    }
}
