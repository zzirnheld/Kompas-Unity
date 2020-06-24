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
        new TemporaryNESWBuff(ServerGame, TriggerRestriction, EndCondition, 
            Target, NBuff, EBuff, SBuff, WBuff);
        ServerEffect.ResolveNextSubeffect();
    }
}
