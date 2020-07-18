using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSubeffect : ServerSubeffect
{
    public int AttackerIndex = -2;

    public override bool Resolve()
    {
        int trueIndex = AttackerIndex < 0 ? AttackerIndex + Effect.Targets.Count : AttackerIndex;
        var attacker = trueIndex < 0 ? null : Effect.Targets[trueIndex];
        var defender = Target;
        if (attacker == null || defender == null) ServerEffect.EffectImpossible();

        ServerGame.Attack(attacker, defender, ServerEffect.ServerController);
        return ServerEffect.ResolveNextSubeffect();
    }
}
