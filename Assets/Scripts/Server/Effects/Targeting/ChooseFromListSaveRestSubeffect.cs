using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChooseFromListSaveRestSubeffect : ChooseFromListSubeffect
{
    public override bool AddListIfLegal(IEnumerable<GameCard> choices)
    {
        //check that there are no elements in choices that aren't in potential targets
        //also check that, if a maximum number to choose has been specified, that many have been chosen
        if ((maxCanChoose > 0 && choices.Count() > maxCanChoose) ||
            choices.Intersect(potentialTargets).Count() != choices.Count())
        {
            RequestTargets();
            return false;
        }

        IEnumerable<GameCard> others = potentialTargets.Except(choices);

        //add the rest of the cards to the list of targets first
        ServerEffect.Rest.AddRange(others);
        //set the value of X to be the number of chosen cards
        ServerEffect.X = choices.Count();
        //add the chosen cards on top
        foreach (var c in choices) ServerEffect.AddTarget(c);

        //everything's cool now, continue resolution
        EffectController.ServerNotifier.AcceptTarget();
        ServerEffect.ResolveNextSubeffect();
        return true;
    }
}
