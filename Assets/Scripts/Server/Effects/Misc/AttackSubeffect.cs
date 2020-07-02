using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSubeffect : ServerSubeffect
{
    public int AttackerIndex = -2;

    private GameCard Attacker
    {
        get
        {
            int trueIndex = AttackerIndex < 0 ? AttackerIndex + Effect.Targets.Count : AttackerIndex;
            return trueIndex < 0 ? null : Effect.Targets[trueIndex];
        }
    }
    private GameCard Defender => Target;

    public override void Resolve()
    {
        ServerGame.Attack(Attacker, Defender, ServerEffect.ServerController);
        ServerEffect.ResolveNextSubeffect();
    }
}
