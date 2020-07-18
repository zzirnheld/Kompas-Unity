using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXBoardRestrictionSubeffect : ServerSubeffect
{
    public CardRestriction cardRestriction;

    public override void Initialize(ServerEffect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        cardRestriction.Initialize(this);
    }

    public override bool Resolve()
    {
        ServerEffect.X = 0;
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                GameCard c = ServerEffect.serverGame.boardCtrl.GetCardAt(i, j);
                if (c == null) continue;
                if (cardRestriction.Evaluate(c)) ServerEffect.X++;
                foreach(GameCard aug in c.Augments)
                {
                    if (cardRestriction.Evaluate(aug)) ServerEffect.X++;
                }
            }
        }
        Debug.Log("Setting X by board restriction to " + ServerEffect.X);
        EffectController.ServerNotifier.NotifyEffectX(ThisCard, ServerEffect.EffectIndex, ServerEffect.X);
        ServerEffect.ResolveNextSubeffect();
    }
}
