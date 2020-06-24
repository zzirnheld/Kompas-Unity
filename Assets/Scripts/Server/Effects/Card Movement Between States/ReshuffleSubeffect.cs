using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReshuffleSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Debug.Log($"Effect reshuffling {Target?.CardName ?? "nothing"}");
        Target.Reshuffle(Target.Owner, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
