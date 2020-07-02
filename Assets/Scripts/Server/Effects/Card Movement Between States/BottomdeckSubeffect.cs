using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomdeckSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Target.Bottomdeck(Target.Owner, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
