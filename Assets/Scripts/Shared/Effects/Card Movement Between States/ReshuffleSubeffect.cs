using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReshuffleSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Reshuffle();
        parent.EffectController.ServerNotifier.NotifyReshuffle(Target);
        parent.ResolveNextSubeffect();
    }
}
