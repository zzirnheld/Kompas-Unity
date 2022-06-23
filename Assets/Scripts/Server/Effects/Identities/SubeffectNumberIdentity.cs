using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;

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
            public INoActivationContextIdentity<ICollection<int>> numbers;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                numbers.Initialize(initializationContext);
            }

            protected override int AbstractItem
                => selector.Apply(numbers.Item);
        }
    }
}