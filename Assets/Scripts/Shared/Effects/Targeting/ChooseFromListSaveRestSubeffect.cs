using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChooseFromListSaveRestSubeffect : ChooseFromListSubeffect
{
    public override bool AddListIfLegal(List<Card> choices)
    {
        //check that there are no elements in choices that aren't in potential targets
        if (choices.Intersect(potentialTargets).Count() != choices.Count)
        {
            RequestTargets();
            return false;
        }

        IEnumerable<Card> others = potentialTargets.Except(choices);

        //add the rest of the cards to the list of targets first
        parent.targets.AddRange(others);
        //set the value of X to be the number of chosen cards
        parent.X = choices.Count;
        //add the chosen cards on top
        parent.targets.AddRange(choices);

        //everything's cool now, continue resolution
        parent.ResolveNextSubeffect();
        return true;
    }
}
