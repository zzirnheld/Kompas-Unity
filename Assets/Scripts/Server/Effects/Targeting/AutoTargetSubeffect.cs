using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class AutoTargetSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction();
            cardRestriction.Initialize(this);
        }

        public override bool IsImpossible() => !Game.Cards.Any(cardRestriction.Evaluate);

        public override Task<ResolutionInfo> Resolve()
        {
            GameCard potentialTarget = null;
            try
            {
                potentialTarget = Game.Cards.SingleOrDefault(cardRestriction.Evaluate);
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