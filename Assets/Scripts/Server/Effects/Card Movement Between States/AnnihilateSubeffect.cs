using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnihilateSubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        if (Target == null) ServerEffect.EffectImpossible();
        else
        {
            if (Game.AnnihilationCtrl.Annihilate(Target, Effect)) ServerEffect.ResolveNextSubeffect();
            else ServerEffect.EffectImpossible();
        }
    }
}
