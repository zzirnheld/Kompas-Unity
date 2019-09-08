using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetThisSpaceSubeffect : Subeffect
{
    public override void Resolve()
    {
        parent.coords.Add(new Vector2Int(parent.thisCard.BoardX, parent.thisCard.BoardY));
        parent.ResolveNextSubeffect();
    }
}
