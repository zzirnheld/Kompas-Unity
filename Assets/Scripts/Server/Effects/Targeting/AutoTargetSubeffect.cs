using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class AutoTargetSubeffect : ServerSubeffect
    {
        public const string Maximum = "Maximum";

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

        public override bool IsImpossible() => !Game.Cards.Any(c => cardRestriction.Evaluate(c, Context));

        public override Task<ResolutionInfo> Resolve()
        {
            GameCard potentialTarget = null;
            try
            {
                var potentialTargets = Game.Cards.Where(c => cardRestriction.Evaluate(c, Context));
                potentialTarget = tiebreakerDirection switch
                {
                    Maximum => potentialTargets.OrderByDescending(c => tiebreakerValue.GetValueOf(c)).First(),
                    _       => potentialTargets.SingleOrDefault(),
                };
            }
            catch (System.InvalidOperationException) 
            {
                Debug.LogError($"More than one card fit the card restriction {cardRestriction} " +
                    $"for the effect {Effect.blurb} of {Source.CardName}");
            }

            if (potentialTarget == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
            
            ServerEffect.AddTarget(potentialTarget);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}