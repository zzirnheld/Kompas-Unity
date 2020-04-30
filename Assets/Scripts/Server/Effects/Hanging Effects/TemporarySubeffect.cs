using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TemporarySubeffect : Subeffect
{
    public TriggerRestriction TriggerRestriction = new TriggerRestriction();
    public TriggerCondition EndCondition;

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        TriggerRestriction.Initialize(this);
    }
}
