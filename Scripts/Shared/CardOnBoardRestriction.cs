using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardOnBoardRestriction : CardRestriction
{
    public enum OnBoardRestrictions { Adjacent, WithinXSpaces}
    public OnBoardRestrictions[] onBoardRestrictions;

    public int xSpaces;

    public override bool Evaluate(Card potentialTarget, bool actuallyTargetThis)
    {
        if (potentialTarget == null) return false;
        foreach(OnBoardRestrictions r in onBoardRestrictions)
        {
            switch (r)
            {
                case OnBoardRestrictions.Adjacent:
                    if(! potentialTarget.IsAdjacentTo(subeffect.parent.thisCard)) return false;
                    break;
                case OnBoardRestrictions.WithinXSpaces:
                    if (!potentialTarget.WithinSlots(xSpaces, subeffect.parent.thisCard)) return false;
                    break;
            }
        }

        return base.Evaluate(potentialTarget, actuallyTargetThis);
    }

}
