using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManySpaces
{
    public class FittingRestriction : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
    {
        public SpaceRestriction restriction;
        public IIdentity<IReadOnlyCollection<Space>> spaces = new ManySpaces.All();

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            restriction.Initialize(initializationContext);
            spaces.Initialize(initializationContext);
        }

        protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => spaces.From(context, secondaryContext)
                .Where(s => restriction.IsValidSpace(s, InitializationContext.effect?.ResolutionContext)).ToList();
    }
}