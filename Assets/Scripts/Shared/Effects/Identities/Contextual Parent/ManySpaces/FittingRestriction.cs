using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManySpacesIdentities
    {
        public class FittingRestriction : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
        {
            public SpaceRestriction restriction;
            public IIdentity<IReadOnlyCollection<Space>> spaces = new All();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => spaces.From(context, secondaryContext)
                    .Where(s => restriction.IsValidSpace(s, InitializationContext.effect?.CurrActivationContext)).ToList();
        }
    }
}