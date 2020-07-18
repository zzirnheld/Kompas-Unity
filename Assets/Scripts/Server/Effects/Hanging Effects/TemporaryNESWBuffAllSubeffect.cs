using System.Collections.Generic;
using System.Linq;

public class TemporaryNESWBuffAllSubeffect : TemporaryCardChangeSubeffect
{
    public int nBuff;
    public int eBuff;
    public int sBuff;
    public int wBuff;

    //default to making sure things are characters before changing their stats
    public CardRestriction cardRestriction = new CardRestriction()
    {
        cardRestrictions = new string[]
        {
            CardRestriction.IsCharacter
        }
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Initialize(this);
    }

    protected override IEnumerable<(HangingEffect, GameCard)> CreateHangingEffects()
    {
        var effs = new List<(HangingEffect, GameCard)>();

        IEnumerable<GameCard> cards = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));

        foreach(var card in cards)
        {
            var temp = new TemporaryNESWBuff(ServerGame, triggerRestriction, endCondition,
                card, nBuff, eBuff, sBuff, wBuff);
            effs.Add((temp, card));
        }

        return effs;
    }
}
