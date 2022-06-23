using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    namespace SubeffectSpaceIdentities
    {
        public class FromActivationContext : SubeffectIdentityBase<Space>
        {
            public IActivationContextIdentity<Space> space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                space.Initialize(initializationContext);
            }

            protected override Space AbstractItem
                => space.From(InitializationContext.subeffect.CurrentContext, default);
        }

        public class PositionOf : SubeffectIdentityBase<Space>
        {
            public INoActivationContextIdentity<GameCardBase> whosePosition;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                whosePosition.Initialize(initializationContext);
            }

            protected override Space AbstractItem => whosePosition.Item.Position;
        }

        public class Target : SubeffectIdentityBase<Space>
        {
            public int index = -1;

            protected override Space AbstractItem => InitializationContext.effect.GetSpace(index);
        }
    }
}