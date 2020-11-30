using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using UnityEngine;

namespace KompasServer.Effects
{
    public class AutoTargetSubeffect : ServerSubeffect
    {
        public CardRestriction cardRestriction = new CardRestriction();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
        }

        public override bool Resolve()
        {
            GameCard potentialTarget = null;
            try
            {
                potentialTarget = Game.Cards.SingleOrDefault(c => cardRestriction.Evaluate(c));
            }
            catch (System.InvalidOperationException) 
            {
                Debug.LogError($"More than one card fit the card restriction {cardRestriction} " +
                    $"for the effect {Effect.blurb} of {Source.CardName}");
            }

            if (potentialTarget == null) return ServerEffect.EffectImpossible();
            else
            {
                ServerEffect.AddTarget(potentialTarget);
                return ServerEffect.ResolveNextSubeffect();
            }
        }
    }
}