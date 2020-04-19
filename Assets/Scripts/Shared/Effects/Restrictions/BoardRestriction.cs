using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardRestriction : CardRestriction
{
    public enum BoardRestrictions {
        Adjacent = 0,
        WithinCSpaces = 1,
        ExactlyXSpaces = 100,
    }
    public BoardRestrictions[] onBoardRestrictions = new BoardRestrictions[0];

    public int cSpaces;

    public override bool Evaluate(Card potentialTarget)
    {
        if (potentialTarget == null) return false;

        foreach(BoardRestrictions r in onBoardRestrictions)
        {
            switch (r)
            {
                case BoardRestrictions.Adjacent:
                    if(!potentialTarget.IsAdjacentTo(Subeffect.ThisCard)) return false;
                    break;
                case BoardRestrictions.WithinCSpaces:
                    if (!potentialTarget.WithinSlots(cSpaces, Subeffect.ThisCard)) return false;
                    break;
                case BoardRestrictions.ExactlyXSpaces:
                    if (potentialTarget.DistanceTo(Subeffect.ThisCard) != Subeffect.Effect.X) return false;
                    break;
            }
        }

        return base.Evaluate(potentialTarget);
    }

}
