using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    namespace SubeffectNumberIdentities
    {
        public class X : SubeffectIdentityBase<int>
        {
            public int multiplier = 1;
            public int modifier = 0;
            public int divisor = 1;

            protected override int AbstractItem
                => (InitializationContext.subeffect.Effect.X * multiplier / divisor) + modifier;
        }
        
        public class Selector : SubeffectIdentityBase<int>
        {
            public INumberSelector selector;
            public IIdentity<IReadOnlyCollection<int>> numbers;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                numbers.Initialize(initializationContext);
            }

            protected override int AbstractItem
                => selector.Apply(numbers.Item);
        }

        public class Arg : SubeffectIdentityBase<int>
        {
            protected override int AbstractItem => InitializationContext.subeffect.Effect.arg;
        }

        public class TargetCount : SubeffectIdentityBase<int>
        {
            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardRestriction?.Initialize(initializationContext);
            }

            private bool Selector(GameCardBase card)
                => cardRestriction?.IsValidCard(card, InitializationContext.effect.CurrActivationContext) ?? true;

            protected override int AbstractItem => InitializationContext.subeffect.Effect.CardTargets.Count(Selector);
        }
    }
}