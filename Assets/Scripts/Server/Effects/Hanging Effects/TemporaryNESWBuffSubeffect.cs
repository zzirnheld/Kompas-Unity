using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryNESWBuffSubeffect : TemporaryCardChangeSubeffect
{
    public int NBuff = 0;
    public int EBuff = 0;
    public int SBuff = 0;
    public int WBuff = 0;

    public int NMultiplier = 0;
    public int EMultiplier = 0;
    public int SMultiplier = 0;
    public int WMultiplier = 0;

    protected override IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects()
    {
        var temp = new TemporaryNESWBuff(ServerGame, TriggerRestriction, EndCondition,
            Target,
            NBuff + Effect.X * NMultiplier,
            EBuff + Effect.X * EMultiplier,
            SBuff + Effect.X * SMultiplier,
            WBuff + Effect.X * WMultiplier);

        return new List<(HangingEffect, GameCard)>() { (temp, Target) };
    }
}
