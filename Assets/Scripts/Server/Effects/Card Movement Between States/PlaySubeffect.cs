using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Play(X, Y, EffectController, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
