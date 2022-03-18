using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class AutoTargetSubeffect : ServerSubeffect
    {
        public const string Maximum = "Maximum";
        public const string Any = "Any";

        public CardRestriction cardRestriction;
        public CardValue tiebreakerValue;
        public string tiebreakerDirection;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction();
            cardRestriction.Initialize(this);
            tiebreakerValue?.Initialize(eff.Source);
        }

        public override bool IsImpossible() => !Game.Cards.Any(c => cardRestriction.IsValidCard(c, Context));

        public override Task<ResolutionInfo> Resolve()
        {
            GameCard potentialTarget = null;
            IEnumerable<GameCard> potentialTargets = null;
            try
            {
                potentialTargets = Game.Cards.Where(c => cardRestriction.IsValidCard(c, Context));
                potentialTarget = tiebreakerDirection switch
                {
                    Maximum => potentialTargets.OrderByDescending(tiebreakerValue.GetValueOf).First(),
                    Any => potentialTargets.First(),
                    _ => potentialTargets.Single(),
                };
            }
            catch (System.InvalidOperationException)
            {
                Debug.LogError($"More than one card fit the card restriction {cardRestriction} " +
                    $"for the effect {Effect.blurb} of {Source.CardName}. Those cards were {potentialTargets}");
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
            }

            ServerEffect.AddTarget(potentialTarget);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}