using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChooseFromListSubeffect : Subeffect
{
    /// <summary>
    /// Restriction that each card must fulfill
    /// </summary>
    public CardRestriction CardRestriction;

    /// <summary>
    /// Restriction that the list collectively must fulfill
    /// </summary>
    public ListRestriction ListRestriction;

    /// <summary>
    /// The maximum number of cards that can be chosen.
    /// If this isn't specified in the json, allow unlimited cards to be chosen.
    /// Represent this with -1
    /// </summary>
    public int MaxCanChoose = -1;

    protected List<Card> potentialTargets;

    protected void RequestTargets()
    {
        EffectController.ServerNotifier.GetChoicesFromList(potentialTargets, MaxCanChoose, this);
    }

    public override void Initialize(Effect eff, int subeffIndex)
    {
        base.Initialize(eff, subeffIndex);
        CardRestriction.Subeffect = this;
    }

    public override void Resolve()
    {
        //TODO: somehow figure out a better way of checking if there exists a valid list?
        //  maybe a method on list restriction that checks?
        //  because otherwise enumerating lists and seeing if at least one fits would be exponential time
        if(!ListRestriction.Evaluate(new List<Card>()))
        {
            Effect.EffectImpossible();
            return;
        }

        potentialTargets = new List<Card>();

        //get all cards that fulfill the cardrestriction
        foreach(KeyValuePair<int, Card> pair in ServerGame.cards)
        {
            if (CardRestriction.Evaluate(pair.Value))
            {
                potentialTargets.Add(pair.Value);
            }
        }

        //if there are no possible targets, declare the effect impossible
        //if you want to continue resolution anyway, add an if impossible check before this subeffect.
        if(potentialTargets.Count == 0)
        {
            Effect.EffectImpossible();
            return;
        }

        RequestTargets();
    }

    public virtual bool AddListIfLegal(IEnumerable<Card> choices)
    {
        //check that there are no elements in choices that aren't in potential targets
        //also check that, if a maximum number to choose has been specified, that many have been chosen
        //also check that the list as a whole is allowable
        bool invalidList = (MaxCanChoose > 0 && choices.Count() > MaxCanChoose) ||
            choices.Intersect(potentialTargets).Count() != choices.Count() ||
            !ListRestriction.Evaluate(choices);

        if (invalidList)
        {
            RequestTargets();
            return false;
        }

        //add all cards in the chosen list to targets
        Effect.targets.AddRange(choices);
        //everything's cool
        Effect.ResolveNextSubeffect();
        return true;
    }

}
