using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaySubeffect : ServerSubeffect
{
    public int NumTimesToDelay = 0;
    public int IndexToResume;
    public string TriggerCondition;
    public TriggerRestriction triggerRestriction = new TriggerRestriction();

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        triggerRestriction.Initialize(this, ThisCard, null);
    }

    public override bool Resolve()
    {
        var eff = new DelayedHangingEffect(ServerGame, triggerRestriction, TriggerCondition,
            NumTimesToDelay, ServerEffect, IndexToResume, EffectController);
        ServerGame.EffectsController.RegisterHangingEffect(TriggerCondition, eff);
        return ServerEffect.EffectImpossible();
    }
}
