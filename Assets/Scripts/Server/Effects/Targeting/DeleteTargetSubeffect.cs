using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTargetSubeffect : ServerSubeffect
{
    public int DeleteIndex = -1;
    public int TrueDeleteIndex { get { return DeleteIndex < 0 ? ServerEffect.targets.Count + DeleteIndex : DeleteIndex; } }

    public override void Resolve()
    {
        if (TrueDeleteIndex < 0)
        {
            ServerEffect.EffectImpossible();
            return;
        }

        ServerEffect.targets.RemoveAt(TrueDeleteIndex);
        ServerEffect.ResolveNextSubeffect();
    }
}
