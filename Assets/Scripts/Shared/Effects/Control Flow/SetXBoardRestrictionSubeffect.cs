using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXBoardRestrictionSubeffect : Subeffect
{
    public BoardRestriction boardRestriction;

    public override void Initialize()
    {
        boardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        Effect.X = 0;
        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                Card c = Effect.serverGame.boardCtrl.GetCardAt(i, j);
                if (c == null) continue;
                if (boardRestriction.Evaluate(c)) Effect.X++;
                foreach(Card aug in c.Augments)
                {
                    if (boardRestriction.Evaluate(aug)) Effect.X++;
                }
            }
        }
        Debug.Log("Setting X to " + Effect.X);
        EffectController.ServerNotifier.NotifyEffectX(ThisCard, Effect.EffectIndex, Effect.X);
        Effect.ResolveNextSubeffect();
    }
}
