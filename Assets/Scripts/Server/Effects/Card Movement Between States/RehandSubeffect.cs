using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RehandSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Rehand(Target.Owner, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
