﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HangingEffect
{
    public TriggerCondition EndCondition { get; }

    private TriggerRestriction triggerRestriction;
    private ServerGame game;

    public HangingEffect(ServerGame game, TriggerRestriction triggerRestriction, TriggerCondition endCondition)
    {
        this.game = game;
        this.triggerRestriction = triggerRestriction;
        this.EndCondition = endCondition;
        game.EffectsController.RegisterHangingEffect(EndCondition, this);
    }

    public virtual bool EndIfApplicable(Card triggerer, IStackable stackTrigger)
    {
        bool end = ShouldEnd(triggerer, stackTrigger);
        if (end) Resolve();
        return end;
    }

    protected virtual bool ShouldEnd(Card triggerer, IStackable stackTrigger)
    {
        return triggerRestriction.Evaluate(triggerer, stackTrigger, triggerRestriction.Subeffect.EffectController);
    }

    protected abstract void Resolve();
}
