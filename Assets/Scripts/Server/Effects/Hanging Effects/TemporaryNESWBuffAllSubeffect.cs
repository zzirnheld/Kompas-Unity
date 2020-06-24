using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryNESWBuffAllSubeffect : TemporarySubeffect
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
        CardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        IEnumerable<GameCard> cards = ServerGame.Cards.Where(c => CardRestriction.Evaluate(c));

        foreach(var card in cards)
        {
            var temp = new TemporaryNESWBuff(ServerGame, TriggerRestriction, EndCondition,
                card, NBuff, EBuff, SBuff, WBuff);
        }

        ServerEffect.ResolveNextSubeffect();
    }
}
