using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThisSpaceTargetSubeffect : Subeffect
{
    public override void Resolve()
    {
        Effect.coords.Add(new Vector2Int(ThisCard.BoardX, ThisCard.BoardY));
        Effect.ResolveNextSubeffect();
    }
}
