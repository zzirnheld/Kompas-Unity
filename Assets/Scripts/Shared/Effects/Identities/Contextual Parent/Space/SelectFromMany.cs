using KompasCore.Effects.Selectors;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities
{
    namespace GamestateSpaceIdentities
    {

        public class SelectFromMany : ContextualParentIdentityBase<Space>
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