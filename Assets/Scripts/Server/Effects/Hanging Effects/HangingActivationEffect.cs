using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingActivationEffect : HangingEffect
{
    private readonly GameCard target;
    private readonly ServerSubeffect source;

    public HangingActivationEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, TriggerCondition endCondition,
        GameCard target, ServerSubeffect source)
        : base(serverGame, triggerRestriction, endCondition)
    {
        this.target = target ?? throw new System.ArgumentNullException("Cannot target a null card for a hanging activation");
        this.source = source ?? throw new System.ArgumentNullException("Cannot make a hanging activation effect from no subeffect");
        serverGame.SetActivated(target, true, source.ServerEffect);
    }

    protected override void Resolve()
    {
        serverGame.SetActivated(target, false, source.ServerEffect);
    }
}
