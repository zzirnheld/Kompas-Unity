using KompasCore.Cards;
using KompasCore.Effects.Selectors;
using System.Collections.Generic;

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

        public class SelectFromMany : NoActivationContextIdentityBase<Space>
        {
            public INoActivationContextIdentity<IReadOnlyCollection<Space>> spaces;
            public Selector selector;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
            }

            protected override Space AbstractItem => selector.Select<Space>(spaces.Item);
        }
    }
}