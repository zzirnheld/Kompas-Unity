using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTargetSubeffect : Subeffect
{
    public int DeleteIndex;

    public override void Resolve()
    {
        if (DeleteIndex < 0) parent.targets.RemoveAt(parent.targets.Count + DeleteIndex);
        else parent.targets.RemoveAt(DeleteIndex);

        parent.ResolveNextSubeffect();
    }
}
