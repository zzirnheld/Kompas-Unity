using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteTargetSubeffect : Subeffect
{
    public int DeleteIndex = -1;
    public int TrueDeleteIndex { get { return DeleteIndex < 0 ? Effect.targets.Count + DeleteIndex : DeleteIndex; } }

    public override void Resolve()
    {
        if (TrueDeleteIndex < 0)
        {
            Effect.EffectImpossible();
            return;
        }

        Effect.targets.RemoveAt(TrueDeleteIndex);
        Effect.ResolveNextSubeffect();
    }
}
