using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryNESWBuffSubeffect : TemporarySubeffect
{
    public int NBuff;
    public int EBuff;
    public int SBuff;
    public int WBuff;

    public override void Resolve()
    {
        if(!(Target is CharacterCard charCard))
        {
            Effect.EffectImpossible();
            return;
        }

        var temp = new TemporaryNESWBuff(ServerGame, TriggerRestriction, EndCondition, 
            charCard, NBuff, EBuff, SBuff, WBuff);
        Effect.ResolveNextSubeffect();
    }
}
