using System.Collections.Generic;
using System.Linq;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
    public class SpacesFitRestriction : TriggerRestrictionElement
    {
        public SpaceRestriction spaceRestriction;
        public IIdentity<IReadOnlyCollection<Space>> spaces;

        public bool any = false;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            spaces.Initialize(initializationContext);
            spaceRestriction.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
        {
            var spacesItem = spaces.From(context, secondaryContext);
            return any
                ? spacesItem.Any(spaceRestriction.IsValidFor(ContextToConsider(context, secondaryContext)))
                : spacesItem.All(spaceRestriction.IsValidFor(ContextToConsider(context, secondaryContext)));
        }
    }
}