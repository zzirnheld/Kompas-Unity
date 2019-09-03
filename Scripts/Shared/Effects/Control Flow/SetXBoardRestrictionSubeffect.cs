using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXBoardRestrictionSubeffect : Subeffect
{
    public BoardRestriction boardRestriction;

    public override void Initialize()
    {
        boardRestriction.subeffect = this;
    }

    public override void Resolve()
    {
        parent.X = 0;
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                Card c = parent.serverGame.boardCtrl.GetCardAt(i, j);
                if (c == null) continue;
                if (boardRestriction.Evaluate(c)) parent.X++;
                foreach(Card aug in c.Augments)
                {
                    if (boardRestriction.Evaluate(aug)) parent.X++;
                }
            }
        }
        parent.ResolveNextSubeffect();
    }
}
