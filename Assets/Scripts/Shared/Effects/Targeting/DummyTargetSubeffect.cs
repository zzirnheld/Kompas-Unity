using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTargetSubeffect : Subeffect
{
    private Restriction restriction;

    public override void Resolve()
    {
        Debug.LogError($"Dummy Subeffects should never try to resolve");
    }

    public DummyTargetSubeffect(Restriction restriction, Effect parent)
    {
        this.restriction = restriction;
        this.parent = parent;
    }
}
