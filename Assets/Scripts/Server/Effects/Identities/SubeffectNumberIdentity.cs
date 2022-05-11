using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectNumberIdentityBase : SubeffectInitializeableBase,
        INoActivationContextNumberIdentity
    {
        protected abstract int AbstractNumber { get; }

        public int Number
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
            public INoActivationContextManyNumbersIdentity numbers;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                numbers.Initialize(initializationContext);
            }

            protected override int AbstractNumber
                => selector.Apply(numbers.Numbers);
        }
    }
}