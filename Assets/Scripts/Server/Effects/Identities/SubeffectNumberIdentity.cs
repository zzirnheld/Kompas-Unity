using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectNumberIdentityBase : SubeffectInitializeableBase,
        INoActivationContextIdentity<int>
    {
        protected abstract int AbstractNumber { get; }

        public int Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractNumber;
            }
        }
    }

    namespace SubeffectNumberIdentities
    {
        public class X : SubeffectNumberIdentityBase
        {
            public int multiplier = 1;
            public int modifier = 0;
            public int divisor = 1;

            protected override int AbstractNumber
                => (InitializationContext.subeffect.Effect.X * multiplier / divisor) + modifier;
        }
        public class Selector : SubeffectNumberIdentityBase
        {
            public INumberSelector selector;
            public INoActivationContextIdentity<ICollection<int>> numbers;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                numbers.Initialize(initializationContext);
            }

            protected override int AbstractNumber
                => selector.Apply(numbers.Item);
        }
    }
}