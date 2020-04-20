using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerChooseXSubeffect : Subeffect
{
    public XRestriction XRest;

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        XRest.Subeffect = this;
    }

    public override void Resolve()
    {
        throw new System.NotImplementedException();
    }
}
