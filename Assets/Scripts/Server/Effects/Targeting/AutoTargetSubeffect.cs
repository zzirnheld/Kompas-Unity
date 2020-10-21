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
            var potentialTarget = Game.Cards.SingleOrDefault(c => cardRestriction.Evaluate(c));
            if (potentialTarget == null) return ServerEffect.EffectImpossible();
            else
            {
                ServerEffect.AddTarget(potentialTarget);
                return ServerEffect.ResolveNextSubeffect();
            }
        }
    }
}