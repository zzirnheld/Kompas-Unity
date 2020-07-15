using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryNESWBuffAllSubeffect : TemporaryCardChangeSubeffect
{
    public int NBuff;
    public int EBuff;
    public int SBuff;
    public int WBuff;

    //default to making sure things are characters before changing their stats
    public CardRestriction CardRestriction = new CardRestriction()
    {
        restrictionsToCheck = new CardRestriction.CardRestrictions[]
        {
            CardRestriction.CardRestrictions.IsCharacter
        }
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        CardRestriction.Initialize(this);
    }

    protected override IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects()
    {
        var effs = new List<(HangingEffect, GameCard)>();

        IEnumerable<GameCard> cards = ServerGame.Cards.Where(c => CardRestriction.Evaluate(c));

        foreach(var card in cards)
        {
            var temp = new TemporaryNESWBuff(ServerGame, TriggerRestriction, EndCondition,
                card, NBuff, EBuff, SBuff, WBuff);
            effs.Add((temp, card));
        }

        return effs;
    }
}
