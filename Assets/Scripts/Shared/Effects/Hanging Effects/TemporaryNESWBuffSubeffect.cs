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
        var temp = new TemporaryNESWBuff(ServerGame, TriggerRestriction, EndCondition, 
            Target as CharacterCard, NBuff, EBuff, SBuff, WBuff);
        Effect.ResolveNextSubeffect();
    }
}
