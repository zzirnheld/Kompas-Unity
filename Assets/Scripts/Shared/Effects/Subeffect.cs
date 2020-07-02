using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Subeffect
{
    public abstract Effect Effect { get; }
    public abstract Player Controller { get; }
    public abstract Game Game { get; }

    public int SubeffIndex { get; protected set; }

    public GameCard Source => Effect.Source;

    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int TargetIndex = -1;

    public int SpaceIndex = -1;

    public GameCard GetTarget(int num)
    {
        int trueIndex = num < 0 ? num + Effect.Targets.Count : num;
        return trueIndex < 0 ? null : Effect.Targets[trueIndex];
    }

    public (int x, int y) GetSpace(int num)
    {
        var trueIndex = num < 0 ? num + Effect.Coords.Count : num;
        return trueIndex < 0 ? (0, 0) : Effect.Coords[trueIndex];
    }

    public GameCard Target => GetTarget(TargetIndex);
    public (int x, int y) Space => GetSpace(SpaceIndex);
}
