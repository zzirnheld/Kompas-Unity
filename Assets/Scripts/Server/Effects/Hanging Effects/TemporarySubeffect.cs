using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TemporarySubeffect : ServerSubeffect
{
    public TriggerRestriction TriggerRestriction = new TriggerRestriction();
    public TriggerCondition EndCondition;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        TriggerRestriction.Initialize(this, ThisCard, null);
    }
}
