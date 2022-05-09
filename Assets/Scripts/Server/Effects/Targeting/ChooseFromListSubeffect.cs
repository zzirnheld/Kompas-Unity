using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KompasCore.Cards;
using KompasCore.Effects;
using Newtonsoft.Json;
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
        public CardRestriction cardRestriction;

        /// <summary>
        /// Restriction that the list collectively must fulfill
        /// </summary>
        public ListRestriction listRestriction;

        public string orderBy = NoOrder;

        protected IEnumerable<GameCard> potentialTargets;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction();
            cardRestriction.Initialize(this);
            listRestriction ??= ListRestriction.Default;
            listRestriction.Initialize(this);
        }

        public override bool IsImpossible()
        {
            var possibleTargets = GetPossibleTargets();
            return !listRestriction.ExistsValidChoice(possibleTargets);
        }

        protected async Task<IEnumerable<GameCard>> RequestTargets()
        {
            string name = Source.CardName;
            string blurb = cardRestriction.blurb;
            int[] targetIds = potentialTargets.Select(c => c.ID).ToArray();
            listRestriction.PrepareForSending(Effect.X);
            Debug.Log($"Potential targets {string.Join(", ", targetIds)}");
            return await ServerPlayer.serverAwaiter.GetCardListTargets(name, blurb, targetIds, JsonConvert.SerializeObject(listRestriction));
        }

        private IEnumerable<GameCard> GetPossibleTargets()
        {
            var possibleTargets = ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, CurrentContext));
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

        protected virtual Task<ResolutionInfo> NoPossibleTargets()
            => Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

        public override async Task<ResolutionInfo> Resolve()
        {
            potentialTargets = GetPossibleTargets();
            //if there's no possible valid combo, throw effect impossible
            if (!listRestriction.ExistsValidChoice(potentialTargets))
            {
                Debug.Log($"List restriction {listRestriction} finds no possible list of targets among potential targets" +
                    $"{string.Join(",", potentialTargets.Select(c => c.CardName))}");
                return await NoPossibleTargets();
            }

            IEnumerable<GameCard> targets = null;
            while (!AddListIfLegal(targets))
            {
                targets = await RequestTargets();
                if (targets == null && ServerEffect.CanDeclineTarget) return ResolutionInfo.Impossible(DeclinedFurtherTargets);
            }

            return ResolutionInfo.Next;
        }

        protected virtual void AddList(IEnumerable<GameCard> choices)
        {
            foreach (var c in choices) ServerEffect.AddTarget(c);
        }

        public bool AddListIfLegal(IEnumerable<GameCard> choices)
        {
            Debug.Log($"Potentially adding list {string.Join(",", choices ?? new List<GameCard>())}");

            if (!listRestriction.IsValidCardList(choices, potentialTargets)) return false;

            //add all cards in the chosen list to targets
            AddList(choices);
            //everything's cool
            ServerPlayer.ServerNotifier.AcceptTarget();
            return true;
        }

    }
}