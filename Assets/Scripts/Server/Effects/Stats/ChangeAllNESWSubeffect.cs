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
    public BoardRestriction BoardRestriction = new BoardRestriction()
    {
        restrictionsToCheck = new CardRestriction.CardRestrictions[]
        {
            CardRestriction.CardRestrictions.IsCharacter,
            CardRestriction.CardRestrictions.Board
        }
    };

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        BoardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        var targets = ServerGame.Cards.Where(c => BoardRestriction.Evaluate(c));
        foreach (GameCard c in targets)
        {
            if (c is CharacterCard charCard) ServerGame.AddToStats(charCard, NMod, EMod, SMod, WMod);
        }

        ServerEffect.ResolveNextSubeffect();
    }
}
