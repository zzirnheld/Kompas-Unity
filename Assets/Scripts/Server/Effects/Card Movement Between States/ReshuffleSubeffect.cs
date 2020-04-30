using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReshuffleSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null)
        {
            ServerEffect.EffectImpossible();
            return;
        }
        Debug.Log($"Effect reshuffling {Target?.CardName ?? "nothing"}");
        ServerGame.Reshuffle(Target, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
