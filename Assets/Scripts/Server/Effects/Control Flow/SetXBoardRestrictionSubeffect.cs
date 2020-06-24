using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXBoardRestrictionSubeffect : ServerSubeffect
{
    public BoardRestriction boardRestriction;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        boardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        ServerEffect.X = 0;
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                GameCard c = ServerEffect.serverGame.boardCtrl.GetCardAt(i, j);
                if (c == null) continue;
                if (boardRestriction.Evaluate(c)) ServerEffect.X++;
                foreach(GameCard aug in c.Augments)
                {
                    if (boardRestriction.Evaluate(aug)) ServerEffect.X++;
                }
            }
        }
        Debug.Log("Setting X by board restriction to " + ServerEffect.X);
        EffectController.ServerNotifier.NotifyEffectX(ThisCard, ServerEffect.EffectIndex, ServerEffect.X);
        ServerEffect.ResolveNextSubeffect();
    }
}
