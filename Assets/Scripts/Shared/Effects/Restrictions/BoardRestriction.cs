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
        Summoned = 200
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
                    if(!potentialTarget.IsAdjacentTo(Subeffect.Source)) return false;
                    break;
                case BoardRestrictions.WithinCSpaces:
                    if (!potentialTarget.WithinSlots(cSpaces, Subeffect.Source)) return false;
                    break;
                case BoardRestrictions.ExactlyXSpaces:
                    if (potentialTarget.DistanceTo(Subeffect.Source) != Subeffect.Effect.X) return false;
                    break;
                    //TODO also allow for summoned avatars, maybe with an overridden property Summoned?
                case BoardRestrictions.Summoned:
                    if (potentialTarget is AvatarCard) return false;
                    break;
            }
        }

        return base.Evaluate(potentialTarget);
    }

}
