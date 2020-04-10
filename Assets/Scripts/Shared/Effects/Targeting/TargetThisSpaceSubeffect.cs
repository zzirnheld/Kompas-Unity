using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSpaceSubeffect : Subeffect
{
    public override void Resolve()
    {
        Effect.coords.Add(new Vector2Int(ThisCard.BoardX, ThisCard.BoardY));
        Effect.ResolveNextSubeffect();
    }
}
