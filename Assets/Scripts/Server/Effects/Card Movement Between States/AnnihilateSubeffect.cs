using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnihilateSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        Game.AnnihilationCtrl.Annihilate(Target, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
