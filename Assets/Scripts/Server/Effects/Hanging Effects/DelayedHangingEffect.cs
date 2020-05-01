using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedHangingEffect : HangingEffect
{
    private int numTimesToDelay;
    private int numTimesDelayed;
    private ServerEffect toResume;
    private int indexToResumeResolution;

    public DelayedHangingEffect(ServerGame game, TriggerRestriction triggerRestriction, TriggerCondition triggerCondition,
        int numTimesToDelay, ServerEffect toResume, int indexToResumeResolution)
        : base(game, triggerRestriction, triggerCondition)
    {
        this.numTimesToDelay = numTimesToDelay;
        this.toResume = toResume;
        this.indexToResumeResolution = indexToResumeResolution;
        numTimesDelayed = 0;
    }

    protected override bool ShouldEnd(Card triggerer, IStackable stackTrigger)
    {
        //first check any other logic
        if (!base.ShouldEnd(triggerer, stackTrigger)) return false;

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
