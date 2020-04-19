using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCardTargetSubeffect : Subeffect
{
    public CardRestriction cardRestriction;

    public override void Resolve()
    {
        throw new System.NotImplementedException("Dummy Card Target Subeffect should never resolve.");
    }

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Subeffect = this;
    }
}
