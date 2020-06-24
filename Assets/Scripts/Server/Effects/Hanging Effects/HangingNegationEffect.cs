using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingNegationEffect : HangingEffect
{
    private readonly GameCard target;
    private readonly ServerSubeffect source;

    public HangingNegationEffect(ServerGame serverGame, TriggerRestriction triggerRestriction, TriggerCondition endCondition,
        GameCard target, ServerSubeffect source)
        : base(serverGame, triggerRestriction, endCondition)
    {
        this.target = target ?? throw new System.ArgumentNullException("Cannot target a null card for a hanging negation");
        this.source = source ?? throw new System.ArgumentNullException("Cannot have a null source subeffect for hanging negatione effect");
        target.SetNegated(true, source.ServerEffect);
    }

    protected override void Resolve()
    {
        target.SetNegated(false, source.ServerEffect);
    }
}
