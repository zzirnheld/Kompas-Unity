using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoardRestriction : CardRestriction
{
    public enum BoardRestrictions {
        Adjacent = 0,
        WithinXSpaces = 1
    }
    public BoardRestrictions[] onBoardRestrictions;

    public int xSpaces;

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
                case BoardRestrictions.WithinXSpaces:
                    if (!potentialTarget.WithinSlots(xSpaces, Subeffect.ThisCard)) return false;
                    break;
            }
        }

        return base.Evaluate(potentialTarget);
    }

}
