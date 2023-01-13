using System.Collections.Generic;
using System.Linq;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class SpaceFitsRestriction : TriggerRestrictionElement
        {
            public SpaceRestriction spaceRestriction;
            public IActivationContextIdentity<Space> space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                space.Initialize(initializationContext);
                spaceRestriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => spaceRestriction.IsValidSpace(space.From(context, secondaryContext), context);
        }
    }

        public class SpacesFitRestriction : TriggerRestrictionElement
        {
            public SpaceRestriction spaceRestriction;
            public IActivationContextIdentity<IReadOnlyCollection<Space>> spaces;

            public bool any = false;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
                spaceRestriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
            {
                var spacesItem = spaces.From(context, secondaryContext);
                return any
                    ? spacesItem.Any(spaceRestriction.IsValidFor(context))
                    : spacesItem.All(spaceRestriction.IsValidFor(context));
            }
        }
}