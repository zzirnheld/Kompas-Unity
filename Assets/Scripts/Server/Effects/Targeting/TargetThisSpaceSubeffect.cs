using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSpaceSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        ServerEffect.Coords.Add((ThisCard.BoardX, ThisCard.BoardY));
        ServerEffect.ResolveNextSubeffect();
    }
}
