using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeAllNESWSubeffect : ServerSubeffect
{
    public int NMod = 0;
    public int EMod = 0;
    public int SMod = 0;
    public int WMod = 0;
    //default to making sure things are characters before changing their stats
    public CardRestriction cardRestriction = new CardRestriction()
    {
        restrictions = new string[]
        {
            CardRestriction.IsCharacter,
            CardRestriction.Board
        }
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Initialize(this);
    }

    public override bool Resolve()
    {
        var targets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));
        foreach (GameCard c in targets)
        {
            c.AddToCharStats(NMod, EMod, SMod, WMod);
        }

        return ServerEffect.ResolveNextSubeffect();
    }
}
