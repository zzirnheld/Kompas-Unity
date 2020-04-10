using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListRestriction
{
    public Subeffect Subeffect;

    public enum ListRestrictions
    {
        CanPayCost = 0 //effect's controller is able to pay the cost of all of them together
    }
    public ListRestrictions[] Restrictions;

    /// <summary>
    /// Checks the list of cards passed into see if they collectively fit a restriction.
    /// </summary>
    /// <param name="cards">The list of cards to collectively evaluate.</param>
    /// <returns><see langword="true"/> if the cards fit all the required restrictions collectively, 
    /// <see langword="false"/> otherwise</returns>
    public bool Evaluate(IEnumerable<Card> cards)
    {
        foreach(var restriction in Restrictions)
        {
            switch (restriction)
            {
                case ListRestrictions.CanPayCost:
                    int totalCost = 0;
                    foreach(var card in cards)
                    {
                        //TODO: check each card's playability, once runes are in?
                        totalCost += card.Cost;
                    }
                    if (Subeffect.EffectController.pips < totalCost) return false;
                    break;
                default:
                    Debug.LogError($"Could not check list restriction {restriction}");
                    return false;
            }
        }
        return true;
    }
}
