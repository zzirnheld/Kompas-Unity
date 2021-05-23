using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class CardTargetSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction;

        public enum TargetType { Normal = 0, Debuff = 1 }
        public TargetType targetType;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction = cardRestriction ?? new CardRestriction();
            cardRestriction.Initialize(this);
        }

        public override bool IsImpossible() => !Game.Cards.Any(cardRestriction.Evaluate);

        protected virtual int[] PotentialTargetIds
            => Game.Cards.Where(cardRestriction.Evaluate).Select(c => c.ID).ToArray();

        protected virtual async Task<GameCard> GetTargets(int[] potentialTargetIds)
        {
            Debug.Log($"Asking for card target among ids {string.Join(", ", potentialTargetIds)}");
            return await ServerPlayer.serverAwaiter.GetCardTarget(Source.CardName, cardRestriction.blurb, potentialTargetIds, null);
        }

        public override async Task<ResolutionInfo> Resolve()
        {
            var potentialTargetIds = PotentialTargetIds;

            //check first that there exist valid targets. if there exist no valid targets, finish resolution here
            if (!potentialTargetIds.Any())
            {
                Debug.Log($"No target exists for {ThisCard.CardName} effect");
                return ResolutionInfo.Impossible(NoValidCardTarget);
            }

            GameCard card = null;
            //while the target is invalid (and null cards, the default value, are invalid), ask the client for a new target.
            //awaiting this means that the potential new target will be evaluated once there is indeed a new target
            while (!AddTargetIfLegal(card))
            {
                card = await GetTargets(potentialTargetIds);
                if (card == null && ServerEffect.CanDeclineTarget) return ResolutionInfo.Impossible(DeclinedFurtherTargets);
            }

            return ResolutionInfo.Next;
        }

        /// <summary>
        /// Check if the card passed is a valid target, and if it is, continue the effect
        /// </summary>
        public virtual bool AddTargetIfLegal(GameCard card)
        {
            //evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
            if (cardRestriction.Evaluate(card))
            {
                ServerEffect.AddTarget(card);
                ServerPlayer.ServerNotifier.AcceptTarget();
                return true;
            }
            else
            {
                Debug.Log($"{card?.CardName} not a valid target for {cardRestriction}");
                return false;
            }
        }
    }
}