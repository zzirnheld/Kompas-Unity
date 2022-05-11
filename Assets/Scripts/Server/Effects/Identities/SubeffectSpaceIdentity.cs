using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectSpaceIdentityBase : SubeffectInitializeableBase,
        INoActivationContextIdentity<Space>
    {
        protected abstract Space AbstractSpace { get; }

        public Space Item
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
            public IActivationContextIdentity<Space> space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                space.Initialize(initializationContext);
            }

            protected override Space AbstractSpace
                => space.From(InitializationContext.subeffect.CurrentContext);
        }

        public class PositionOf : SubeffectSpaceIdentityBase
        {
            public INoActivationContextIdentity<GameCardBase> whosePosition;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                whosePosition.Initialize(initializationContext);
            }

            protected override Space AbstractSpace => whosePosition.Item.Position;
        }
    }
}