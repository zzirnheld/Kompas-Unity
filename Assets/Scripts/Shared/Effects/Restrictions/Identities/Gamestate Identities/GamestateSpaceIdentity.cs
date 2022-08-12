using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    namespace GamestateSpaceIdentities
    {
        public class PositionOf : NoActivationContextIdentityBase<Space>
        {
            public INoActivationContextIdentity<GameCardBase> card;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(InitializationContext);
                card.Initialize(InitializationContext);
            }

            protected override Space AbstractItem => card.Item.Position;
        }
    }
}