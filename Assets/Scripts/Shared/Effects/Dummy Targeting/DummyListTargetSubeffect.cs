using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyListTargetSubeffect : Subeffect
{
    /// <summary>
    /// Restriction that each card must fulfill
    /// </summary>
    public CardRestriction CardRestriction;

    /// <summary>
    /// Restriction that the list collectively must fulfill
    /// </summary>
    public ListRestriction ListRestriction;

    public override void Resolve()
    {
        throw new System.NotImplementedException(("Dummy List Target Subeffect should never resolve."));
    }

    public override void Initialize()
    {
        CardRestriction.Subeffect = this;
        ListRestriction.Subeffect = this;
    }
}
