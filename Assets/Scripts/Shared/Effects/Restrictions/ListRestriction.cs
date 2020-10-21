using System.Collections.Generic;
using UnityEngine;
using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class ListRestriction
    {
        public Subeffect Subeffect { get; private set; }

        public const string CanPayCost = "Can Pay Cost"; // 1 //effect's controller is able to pay the cost of all of them together
        public const string MinCanChoose = "Min Can Choose";
        public const string MaxCanChoose = "Max Can Choose";

        public string[] listRestrictions = new string[0];

        /// <summary>
        /// The maximum number of cards that can be chosen.
        /// Default: one card can be chosen
        /// </summary>
        public int maxCanChoose = 1;

        /// <summary>
        /// The minimum number of cards that must be chosen.
        /// If is < 0, gets set to maxCanChoose
        /// Default: one card must be chosen
        /// </summary>
        public int minCanChoose = 1;

        public static ListRestriction Default => new ListRestriction()
        {
            listRestrictions = new string[]
            {
                MinCanChoose, MaxCanChoose
            }
        };

        public void Initialize(Subeffect subeffect)
        {
            Subeffect = subeffect;
            if (minCanChoose < 0) minCanChoose = maxCanChoose;
        }

        private bool EvaluateRestriction(string restriction, IEnumerable<GameCard> cards)
        {
            switch (restriction)
            {
                case CanPayCost:
                    int totalCost = 0;
                    foreach (var card in cards) totalCost += card.Cost;
                    return Subeffect.Controller.Pips >= totalCost;
                case MinCanChoose: return cards.Count() >= minCanChoose;
                case MaxCanChoose: return cards.Count() <= maxCanChoose;
                default: throw new System.ArgumentException($"Invalid list restriction {restriction}", "restriction");
            }
        }

        private bool EvaluateRestrictionWithDebug(string restriction, IEnumerable<GameCard> cards)
        {
            bool valid = EvaluateRestriction(restriction, cards);
            if (!valid) Debug.Log($"Invalid list of cards {string.Join(", ", cards.Select(c => c.CardName))} flouts list restriction {restriction}");
            return valid;
        }

        /// <summary>
        /// Checks the list of cards passed into see if they collectively fit a restriction.
        /// </summary>
        /// <param name="choices">The list of cards to collectively evaluate.</param>
        /// <returns><see langword="true"/> if the cards fit all the required restrictions collectively, 
        /// <see langword="false"/> otherwise</returns>
        public bool Evaluate(IEnumerable<GameCard> choices, IEnumerable<GameCard> potentialTargets)
        {
            if (choices.Except(potentialTargets).Any())
            {
                /*Debug.Log($"Some cards in list of choices {string.Join(",", choices.Select(c => c.CardName))}" +
                    $" don't appear in the list of potential targets {string.Join(",", potentialTargets.Select(c => c.CardName))}.");*/
                Debug.Log("Some choices don't appear in the list of potential targets");
                return false;
            }
            return listRestrictions.All(r => EvaluateRestrictionWithDebug(r, choices));
        }

        private bool EvaluateValidListChoice(string restriction, IEnumerable<GameCard> potentialTargets)
        {
            switch (restriction)
            {
                case CanPayCost:
                    int costAccumulation = 0;
                    int i = 0;
                    foreach(var card in potentialTargets.OrderBy(c => c.Cost))
                    {
                        if (i > minCanChoose) break;
                        costAccumulation += card.Cost;
                        i++;
                    }
                    if(i < minCanChoose) return false;
                    return costAccumulation <= Subeffect.Controller.Pips;
                case MinCanChoose: return potentialTargets.Count() >= minCanChoose;
                case MaxCanChoose: return true;
                default: throw new System.ArgumentException($"Invalid list restriction {restriction}", "restriction");
            }
        }

        public bool ExistsValidChoice(IEnumerable<GameCard> potentialTargets) 
            => listRestrictions.All(r => EvaluateValidListChoice(r, potentialTargets));
    }
}