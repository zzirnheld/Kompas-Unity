using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBoardTargetSubeffect : DummySubeffect
{
    public BoardRestriction boardRestriction;

    public override void Initialize(ClientEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        boardRestriction.Initialize(this);
    }
}
