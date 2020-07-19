using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTargetSubeffect : ServerSubeffect
{
    public int DeleteIndex = -1;
    public int TrueDeleteIndex { get { return DeleteIndex < 0 ? ServerEffect.Targets.Count + DeleteIndex : DeleteIndex; } }

    public override bool Resolve()
    {
        if (TrueDeleteIndex < 0 || TrueDeleteIndex >= ServerEffect.Targets.Count)
            return ServerEffect.EffectImpossible();

        ServerEffect.Targets.RemoveAt(TrueDeleteIndex);
        return ServerEffect.ResolveNextSubeffect();
    }
}
