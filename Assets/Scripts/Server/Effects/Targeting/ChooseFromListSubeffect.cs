using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class ChooseFromListSubeffect : ServerSubeffect
    {
        public const string NoOrder = "No Order";
        public const string Closest = "Closest";

        /// <summary>
        /// Restriction that each card must fulfill
        /// </summary>
        public CardRestriction cardRestriction = new CardRestriction();

        /// <summary>
        /// Restriction that the list collectively must fulfill
        /// </summary>
        public ListRestriction listRestriction = new ListRestriction();

        /// <summary>
        /// The maximum number of cards that can be chosen.
        /// If this isn't specified in the json, allow unlimited cards to be chosen.
        /// Represent this with -1
        /// </summary>
        public int maxCanChoose = -1;

        /// <summary>
        /// The minimum number of cards that must be chosen.
        /// If is < 0, gets set to maxCanChoose
        /// </summary>
        public int minCanChoose = -1;

        public string orderBy = NoOrder;

        protected IEnumerable<GameCard> potentialTargets;

        protected void RequestTargets()
            => ServerPlayer.ServerNotifier.GetChoicesFromList(potentialTargets, maxCanChoose, this);

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
            listRestriction.Subeffect = this;
            if (minCanChoose < 0) minCanChoose = maxCanChoose;
        }

        private IEnumerable<GameCard> GetPossibleTargets()
        {
            var possibleTargets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));
            if (!possibleTargets.Any()) return new GameCard[0];

            switch (orderBy)
            {
                case NoOrder: return possibleTargets;
                case Closest: 
                    int minDist = possibleTargets.Min(c => c.DistanceTo(Source));
                    return possibleTargets.Where(c => c.DistanceTo(Source) == minDist);
                default: throw new System.ArgumentException($"Invalid ordering in choose from list");
            }
        }

        public override bool Resolve()
        {
            //TODO: somehow figure out a better way of checking if there exists a valid list?
            //  maybe a method on list restriction that checks?
            //  because otherwise enumerating lists and seeing if at least one fits would be exponential time
            if (!listRestriction.Evaluate(new List<GameCard>())) 
                return ServerEffect.EffectImpossible();

            potentialTargets = GetPossibleTargets();

            //if there are not enough possible targets, declare the effect impossible
            //if you want to continue resolution anyway, add an if impossible check before this subeffect.
            if (potentialTargets.Count() < minCanChoose || potentialTargets.Count() < 1)
                return ServerEffect.EffectImpossible();

            RequestTargets();
            return false;
        }

        protected virtual void AddList(IEnumerable<GameCard> choices)
        {
            foreach (var c in choices) ServerEffect.AddTarget(c);
        }

        public bool AddListIfLegal(IEnumerable<GameCard> choices)
        {
            /*
             * Check that all choices were potential targets.
             * Check that the minimum number of items, if any, has been surpassed.
             * Check that the maximum number of items, if any, hasn't been exceeded.
             * Check that all choices were distinct.
             * Check that the list restriction is satisfied.
             */
            bool validList = choices.All(c => potentialTargets.Contains(c)) &&
                (minCanChoose < 0 || choices.Count() >= minCanChoose) &&
                (maxCanChoose < 0 || choices.Count() <= maxCanChoose) &&
                choices.Distinct().Count() == choices.Count() &&
                listRestriction.Evaluate(choices);

            if (!validList)
            {
                RequestTargets();
                return false;
            }

            //add all cards in the chosen list to targets
            AddList(choices);
            //everything's cool
            EffectController.ServerNotifier.AcceptTarget();
            return ServerEffect.ResolveNextSubeffect();
        }

    }
}