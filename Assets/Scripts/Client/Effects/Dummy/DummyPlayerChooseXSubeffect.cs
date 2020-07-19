using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerChooseXSubeffect : DummySubeffect
{
    public XRestriction XRest;

    public override void Initialize(ClientEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        XRest.Initialize(this);
    }
}
