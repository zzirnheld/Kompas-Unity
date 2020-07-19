using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBoardTargetSubeffect : DummySubeffect
{
    public CardRestriction cardRestriction;

    public override void Initialize(ClientEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Initialize(this);
    }
}
