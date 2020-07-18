using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeAllNESWSubeffect : ServerSubeffect
{
    public int nMod = 0;
    public int eMod = 0;
    public int sMod = 0;
    public int wMod = 0;
    //default to making sure things are characters before changing their stats
    public CardRestriction cardRestriction = new CardRestriction()
    {
        cardRestrictions = new string[]
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
            c.AddToCharStats(nMod, eMod, sMod, wMod);
        }

        return ServerEffect.ResolveNextSubeffect();
    }
}
