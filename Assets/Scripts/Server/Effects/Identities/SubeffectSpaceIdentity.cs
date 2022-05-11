using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectSpaceIdentityBase : SubeffectInitializeableBase,
        INoActivationContextSpaceIdentity
    {
        protected abstract Space AbstractSpace { get; }

        public Space Space
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractSpace;
            }
        }
    }

    namespace SubeffectSpaceIdentities
    {
        public class FromActivationContext : SubeffectSpaceIdentityBase
        {
            public IActivationContextSpaceIdentity space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                space.Initialize(initializationContext);
            }

            protected override Space AbstractSpace
                => space.SpaceFrom(InitializationContext.subeffect.CurrentContext);
        }

        public class PositionOf : SubeffectSpaceIdentityBase
        {
            public INoActivationContextCardIdentity whosePosition;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                whosePosition.Initialize(initializationContext);
            }

            protected override Space AbstractSpace => whosePosition.Card.Position;
        }
    }
}