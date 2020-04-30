using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisSpaceTargetSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        ServerEffect.coords.Add(new Vector2Int(ThisCard.BoardX, ThisCard.BoardY));
        ServerEffect.ResolveNextSubeffect();
    }
}
