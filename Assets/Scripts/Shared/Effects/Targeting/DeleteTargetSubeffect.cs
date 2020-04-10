using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTargetSubeffect : Subeffect
{
    public int DeleteIndex;

    public override void Resolve()
    {
        if (DeleteIndex < 0) Effect.targets.RemoveAt(Effect.targets.Count + DeleteIndex);
        else Effect.targets.RemoveAt(DeleteIndex);

        Effect.ResolveNextSubeffect();
    }
}
