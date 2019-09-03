using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXTargetSSubeffect : Subeffect
{
    public override void Resolve()
    {
        if (!(Target is CharacterCard charTarget))
        {
            parent.EffectImpossible();
            return;
        }

        parent.X = charTarget.S;
        parent.ResolveNextSubeffect();
    }
}
