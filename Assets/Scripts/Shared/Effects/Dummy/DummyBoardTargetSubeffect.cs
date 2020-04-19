using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBoardTargetSubeffect : Subeffect
{
    public BoardRestriction boardRestriction;

    public override void Resolve()
    {
        throw new System.NotImplementedException("Dummy Board Target Subeffect should never resolve.");
    }

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        boardRestriction.Subeffect = this;
    }
}
