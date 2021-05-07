using System.Collections.Generic;
using UnityEngine;
using KompasCore.Cards;
using System.Linq;
using System.Text;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class ListRestriction
    {
        public Subeffect Subeffect { get; private set; }

        //if i end up living towards the heat death of the universe,
        //i will refactor this to instead be objects that get deserialized.
        //they will probably be tiny little classes at the bottom of this.
        //dang. that actually makes it sound mostly trivial.
        #region restrictions
        public const string MinCanChoose = "Min Can Choose";
        public const string MaxCanChoose = "Max Can Choose";

        public const string CanPayCost = "Can Pay Cost"; // 1 //effect's controller is able to pay the cost of all of them together
        public const string DistinctCosts = "Distinct Costs";
        public const string MaxOfX = "Max Can Choose: X";
        #endregion restrictions

        public string[] listRestrictions = new string[0];

        /// <summary>
        /// A quick little property that tells you whether the list restriction has a limit to how many can be chosen.
        /// </summary>
        public bool HasMax => listRestrictions.Contains(MaxCanChoose) || listRestrictions.Contains(MaxOfX);

        /// <summary>
        /// Quick little property that informs you whether the list restriction has minimum
        /// </summary>
        public bool HasMin => listRestrictions.Contains(MinCanChoose);

        /// <summary>
        /// Quick little method that tells you if you have selected enough items.
        /// </summary>
        /// <param name="count">Number of items currently selected</param>
        /// <returns>Whether the number of items currently selected is enough.</returns>
        public bool HaveEnough(int count) => !HasMin || count >= minCanChoose;

        /// <summary>
        /// The maximum number of cards that can be chosen.
        /// Default: one card can be chosen
        /// </summary>
        public int maxCanChoose = 1;

        /// <summary>
        /// The minimum number of cards that must be chosen.
        /// If is < 0, gets set to maxCanChoose
        /// Default: set to maxCanChoose
        /// </summary>
        public int minCanChoose = -1;

        /// <summary>
        /// Default ListRestriction. <br></br>
        /// Includes a max and min of 1 card.
        /// </summary>
        public static ListRestriction Default => new ListRestriction()
        {
            listRestrictions = new string[] { MinCanChoose, MaxCanChoose }
        };

        /// <summary>
        /// Default ListRestriction Json. <br></br>
        /// A json representation of <see cref="Default"/>
        /// </summary>
        public static readonly string DefaultJson = JsonUtility.ToJson(Default);

        /// <summary>
        /// You can read, you know what this does.
        /// Initializes the list restriction to know who its daddy is, and make any shtuff match up
        /// </summary>
        /// <param name="subeffect"></param>
        public void Initialize(Subeffect subeffect)
        {
            Subeffect = subeffect;
            if (minCanChoose < 0 && listRestrictions.Contains(MaxCanChoose))
                minCanChoose = maxCanChoose;
        }

        /// <summary>
        /// Prepares the list restriction to be sent to a player alongside a get card target request.
        /// This exists in case I ever need to add information, 
        /// so I can make the compiler tell me where else needs to provide information.
        /// </summary>
        /// <param name="x">The value of x to use, in case the list restriction cares about X.</param>
        public void PrepareForSending(int x)
        {
            if (listRestrictions.Contains(MaxOfX)) maxCanChoose = x;
        }

        private bool EvaluateRestriction(string restriction, IEnumerable<GameCard> cards)
        {
            switch (restriction)
            {
                case MinCanChoose: return cards.Count() >= minCanChoose;
                case MaxCanChoose: return cards.Count() <= maxCanChoose;
                case CanPayCost:
                    int totalCost = 0;
                    foreach (var card in cards) totalCost += card.Cost;
                    return Subeffect.Controller.Pips >= totalCost;
                case DistinctCosts:
                    return cards.Select(c => c.Cost).Distinct().Count() == cards.Count();
                case MaxOfX: return cards.Count() <= Subeffect.Effect.X;
                default: throw new System.ArgumentException($"Invalid list restriction {restriction}", "restriction");
            }
        }

        private bool EvaluateRestrictionWithDebug(string restriction, IEnumerable<GameCard> cards)
        {
            bool valid = EvaluateRestriction(restriction, cards);
            if (!valid) Debug.Log($"Invalid list of cards {string.Join(", ", cards.Select(c => c.CardName))} " +
                $"flouts list restriction {restriction}");
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
            if (choices == null) return false;

            if (choices.Except(potentialTargets).Any())
            {
                Debug.Log($"Some cards in list of choices {string.Join(",", choices.Select(c => c.CardName))}" +
                    $" don't appear in the list of potential targets {string.Join(",", potentialTargets.Select(c => c.CardName))}.");
                return false;
            }
            
            return listRestrictions.All(r => EvaluateRestrictionWithDebug(r, choices));
        }

        private bool RestrictionAllowsValidChoice(string restriction, IEnumerable<GameCard> potentialTargets)
        {
            switch (restriction)
            {
                case CanPayCost:
                    int costAccumulation = 0;
                    int i = 1;
                    foreach(var card in potentialTargets.OrderBy(c => c.Cost))
                    {
                        if (i > minCanChoose) break;
                        costAccumulation += card.Cost;
                        i++;
                    }
                    if(i < minCanChoose) return false;
                    return costAccumulation <= Subeffect.Controller.Pips;
                case MinCanChoose: return potentialTargets.Count() >= minCanChoose;
                case DistinctCosts: return potentialTargets.Select(c => c.Cost).Distinct().Count() > (HasMin ? 0 : minCanChoose);
                case MaxOfX:
                case MaxCanChoose: 
                    return true;
                default: throw new System.ArgumentException($"Invalid list restriction {restriction}", "restriction");
            }
        }

        public bool ExistsValidChoice(IEnumerable<GameCard> potentialTargets) 
            => listRestrictions.All(r => RestrictionAllowsValidChoice(r, potentialTargets));

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("List Restriction:");
            foreach (var r in listRestrictions) sb.AppendLine(r);
            return sb.ToString();
        }
    }
}