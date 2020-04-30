using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaySubeffect : ServerSubeffect
{
    public int NumTimesToDelay = 0;
    public int IndexToResume;
    public TriggerCondition TriggerCondition;
    public TriggerRestriction TriggerRestriction = new TriggerRestriction();

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        TriggerRestriction.Initialize(this);
    }

    public override void Resolve()
    {
        var eff = new DelayedHangingEffect(ServerGame, TriggerRestriction, TriggerCondition,
            NumTimesToDelay, ServerEffect, IndexToResume);
        ServerGame.RegisterHangingEffect(TriggerCondition, eff);
        ServerEffect.EffectImpossible();
    }
}
