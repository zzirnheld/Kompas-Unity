using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyListTargetSubeffect : DummySubeffect
{
    /// <summary>
    /// Restriction that each card must fulfill
    /// </summary>
    public CardRestriction CardRestriction;

    /// <summary>
    /// Restriction that the list collectively must fulfill
    /// </summary>
    public ListRestriction ListRestriction;

    public override void Initialize(ClientEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        CardRestriction.Subeffect = this;
        ListRestriction.Subeffect = this;
    }
}
