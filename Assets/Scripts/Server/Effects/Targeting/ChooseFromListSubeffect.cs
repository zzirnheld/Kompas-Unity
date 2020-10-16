using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

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
        public ListRestriction listRestriction = ListRestriction.Default;

        public string orderBy = NoOrder;

        protected IEnumerable<GameCard> potentialTargets;

        protected void RequestTargets()
            => ServerPlayer.ServerNotifier.GetChoicesFromList(potentialTargets, listRestriction.maxCanChoose, this);

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
            listRestriction.Initialize(this);
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

        protected virtual bool NoPossibleTargets() => ServerEffect.EffectImpossible();

        public override bool Resolve()
        {
            potentialTargets = GetPossibleTargets();
            //if there's no possible valid combo, throw effect impossible
            if (!listRestriction.ExistsValidChoice(potentialTargets))
                return NoPossibleTargets();

            Debug.Log($"Potential targets {string.Join(", ", potentialTargets.Select(t => t.CardName))}");

            RequestTargets();
            return false;
        }

        protected virtual void AddList(IEnumerable<GameCard> choices)
        {
            foreach (var c in choices) ServerEffect.AddTarget(c);
        }

        public bool AddListIfLegal(IEnumerable<GameCard> choices)
        {
            if (!listRestriction.Evaluate(choices, potentialTargets))
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