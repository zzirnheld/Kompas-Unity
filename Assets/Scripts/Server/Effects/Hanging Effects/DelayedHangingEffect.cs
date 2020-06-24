using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedHangingEffect : HangingEffect
{
    private readonly int numTimesToDelay;
    private int numTimesDelayed;
    private ServerEffect toResume;
    private readonly int indexToResumeResolution;

    public DelayedHangingEffect(ServerGame game, TriggerRestriction triggerRestriction, TriggerCondition triggerCondition,
        int numTimesToDelay, ServerEffect toResume, int indexToResumeResolution)
        : base(game, triggerRestriction, triggerCondition)
    {
        this.numTimesToDelay = numTimesToDelay;
        this.toResume = toResume;
        this.indexToResumeResolution = indexToResumeResolution;
        numTimesDelayed = 0;
    }

    protected override bool ShouldEnd(GameCard cardTrigger, IStackable stackTrigger, Player triggerer, int? x, (int x, int y)? space)
    {
        //first check any other logic
        if (!base.ShouldEnd(cardTrigger, stackTrigger, triggerer, x, space)) return false;

        //if it should otherwise be fine, but we haven't waited enough times, delay further
        if (numTimesDelayed < numTimesToDelay)
        {
            numTimesDelayed++;
            return false;
        }

        numTimesDelayed = 0;
        return true;
    }

    protected override void Resolve()
    {
        toResume.ResolveSubeffect(indexToResumeResolution);
    }
}
