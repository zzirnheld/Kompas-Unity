using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDummySubeffect : ServerSubeffect
{
    public TriggerDummySubeffect(ServerEffect eff)
    {
        this.ServerEffect = eff;
    }

    public override bool Resolve()
    {
        throw new System.NotImplementedException("Trigger Dummy Subeffect only exists so that card restriction of trigger restriction has an effect to point to." +
            "It should never resolve.");
    }
}
