using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnihilateSubeffect : CardChangeStateSubeffect
{
    public override bool Resolve()
    {
        if (Target == null) return ServerEffect.EffectImpossible();
        else
        {
            if (Game.annihilationCtrl.Annihilate(Target, Effect)) return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}
