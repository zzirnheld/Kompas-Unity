using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KompasCore.Cards;
using KompasCore.Effects;
using Newtonsoft.Json;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    public class ChooseFromList : ServerSubeffect
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
            cardRestriction.Initialize(DefaultInitializationContext);
            listRestriction ??= ListRestriction.Default;
            listRestriction.Initialize(DefaultInitializationContext);
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
            Debug.Log($"Potential targets {string.Join(", ", targetIds)}");
            return await ServerPlayer.awaiter.GetCardListTargets(name, blurb, targetIds, JsonConvert.SerializeObject(listRestriction));
        }

        protected virtual bool IsValidTarget(GameCard card) => cardRestriction.IsValid(card, ResolutionContext);

        private IEnumerable<GameCard> GetPossibleTargets()
        {
            var possibleTargets = ServerGame.Cards.Where(IsValidTarget);
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
            listRestriction.PrepareForSending(Effect.X);
            //if there's no possible valid combo, throw effect impossible
            if (!listRestriction.ExistsValidChoice(potentialTargets))
            {
                Debug.Log($"List restriction {listRestriction} finds no possible list of targets among potential targets" +
                    $"{string.Join(",", potentialTargets.Select(c => c.CardName))}");
                return await NoPossibleTargets();
            }

            //If there's no potential targets, but no targets is a valid choice, then just go to the next effect
            if (!potentialTargets.Any())
            {
                Debug.Log("An empty list of targets was a valid choice, but there's no targets that can be chosen. Skipping to next effect...");
                return ResolutionInfo.Next;
            }
            else if (listRestriction.HasMax && listRestriction.maxCanChoose == 0)
            {
                Debug.Log("An empty list of targets was a valid choice, and the max to be chosen was 0. Skipping to next effect...");
                return ResolutionInfo.Next;
            }

            IEnumerable<GameCard> targets = null;
            do
            {
                targets = await RequestTargets();
                if (targets == null && ServerEffect.CanDeclineTarget) return ResolutionInfo.Impossible(DeclinedFurtherTargets);
            } while (!AddListIfLegal(targets));

            return ResolutionInfo.Next;
        }

        protected virtual void AddList(IEnumerable<GameCard> choices)
        {
            foreach (var c in choices) ServerEffect.AddTarget(c);
        }

        public bool AddListIfLegal(IEnumerable<GameCard> choices)
        {
            Debug.Log($"Potentially adding list {string.Join(",", choices ?? new List<GameCard>())}");

            if (!listRestriction.IsValidList(choices, potentialTargets)) return false;

            //add all cards in the chosen list to targets
            AddList(choices);
            //everything's cool
            ServerPlayer.notifier.AcceptTarget();
            return true;
        }

    }
}