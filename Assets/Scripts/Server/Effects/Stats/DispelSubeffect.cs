using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispelSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        //this checks if Target is null, and if non-null, if it's a spell
        if(!(Target is SpellCard spellCard))
        {
            ServerEffect.EffectImpossible();
            return;
        }
        ServerGame.Dispel(spellCard, ServerEffect);
        ServerEffect.ResolveNextSubeffect();
    }
}
