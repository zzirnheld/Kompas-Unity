using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSpellCSubeffect : Subeffect
{
    public int XModifier = 0;
    public int XMultiplier = 0;
    public int XDivisor = 1;

    public override void Resolve()
    {
        if(!(Target is SpellCard spellTarget))
        {
            Effect.EffectImpossible();
            return;
        }

        spellTarget.C = Effect.X * XMultiplier / XDivisor + XModifier;
        Effect.ResolveNextSubeffect();
    }
}
