using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TemporaryEffect
{
    public TriggerCondition EndCondition { get; }

    private TriggerRestriction triggerRestriction;
    private Game game;

    public TemporaryEffect(Game game, TriggerRestriction triggerRestriction, TriggerCondition endCondition)
    {
        this.game = game;
        this.triggerRestriction = triggerRestriction;
        this.EndCondition = endCondition;
        game.RegisterTemporaryEffect(EndCondition, this);
    }

    public bool EndIfApplicable(Card triggerer, IStackable stackTrigger)
    {
        bool end = triggerRestriction.Evaluate(triggerer, stackTrigger);
        if (end) Undo();
        return end;
    }
    protected abstract void Undo();
}
