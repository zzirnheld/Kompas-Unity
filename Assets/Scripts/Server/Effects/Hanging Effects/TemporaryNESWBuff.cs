using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryNESWBuff : HangingEffect
{
    private readonly CharacterCard buffRecipient;
    private readonly int nBuff = 0;
    private readonly int eBuff = 0;
    private readonly int sBuff = 0;
    private readonly int wBuff = 0;

    public TemporaryNESWBuff(Game game, TriggerRestriction triggerRestriction, TriggerCondition EndCondition, CharacterCard buffRecipient, int nBuff, int eBuff, int sBuff, int wBuff) 
        : base(game, triggerRestriction, EndCondition)
    {
        this.buffRecipient = buffRecipient;
        this.nBuff = nBuff;
        this.eBuff = eBuff;
        this.sBuff = sBuff;
        this.wBuff = wBuff;

        buffRecipient.game.AddToStats(buffRecipient, nBuff, eBuff, sBuff, wBuff);
    }

    protected override void Resolve() => buffRecipient.game.AddToStats(buffRecipient, -1 * nBuff, -1 * eBuff, -1 * sBuff, -1 * wBuff);
}
