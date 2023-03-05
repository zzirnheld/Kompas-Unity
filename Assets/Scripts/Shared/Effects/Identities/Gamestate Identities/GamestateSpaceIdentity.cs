using KompasCore.Cards;
using KompasCore.Effects.Selectors;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities
{
    namespace GamestateSpaceIdentities
    {
        public class PositionOf : ContextualIdentityBase<Space>
        {
            public IIdentity<GameCardBase> card;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(InitializationContext);
                card.Initialize(InitializationContext);
            }

            protected override Space AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => card.From(context, secondaryContext).Position;
        }

        public class SelectFromMany : ContextualIdentityBase<Space>
        {
            public IIdentity<IReadOnlyCollection<Space>> spaces;
            public ISelector<Space> selector;// = new RandomSelector<Space>();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
            }

            protected override Space AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => selector.Select(spaces.From(context, secondaryContext));
        }
    }
}