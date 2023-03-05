using System.Collections.Generic;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManySpacesIdentities
    {
        public class AdjacentSpaces : ContextualIdentityBase<IReadOnlyCollection<Space>>
        {
            public IIdentity<Space> adjacentTo;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                adjacentTo.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => adjacentTo.From(context, secondaryContext).AdjacentSpaces;
        }
    }
}