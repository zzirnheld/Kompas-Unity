using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetTargetsControllerSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (Target == null) return ServerEffect.EffectImpossible();
            Effect.players.Add(Target.Controller);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}