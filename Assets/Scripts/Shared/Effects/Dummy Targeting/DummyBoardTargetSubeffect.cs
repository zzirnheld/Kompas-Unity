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
}
