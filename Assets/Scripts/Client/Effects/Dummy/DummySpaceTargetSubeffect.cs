using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpaceTargetSubeffect : DummySubeffect
{
    public SpaceRestriction spaceRestriction;

    public override void Initialize(ClientEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        spaceRestriction.Initialize(this);
    }
}
