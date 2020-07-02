using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisSpaceTargetSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        ServerEffect.Coords.Add((ThisCard.BoardX, ThisCard.BoardY));
        ServerEffect.ResolveNextSubeffect();
    }
}
