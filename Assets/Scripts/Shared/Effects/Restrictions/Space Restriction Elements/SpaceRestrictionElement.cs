using KompasCore.Effects.Identities;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class SpaceRestrictionElement : ContextInitializeableBase, IContextInitializeable
    {
        public bool IsValidSpace(Space space, IResolutionContext context)
        {
            ComplainIfNotInitialized();
            return AbstractIsValidSpace(space, context);
        }

        protected abstract bool AbstractIsValidSpace(Space space, IResolutionContext context);
    }

    namespace SpaceRestrictionElements
    {
        public class Not : SpaceRestrictionElement
        {
            public SpaceRestrictionElement negated;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                negated.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, IResolutionContext context)
                => !negated.IsValidSpace(space, context);
        }

        public class AnyOf : SpaceRestrictionElement
        {
            public SpaceRestrictionElement[] restrictions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var r in restrictions) r.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, IResolutionContext context)
                => restrictions.Any(r => r.IsValidSpace(space, context));
        }

        public class Empty : SpaceRestrictionElement
        {
            protected override bool AbstractIsValidSpace(Space space, IResolutionContext context)
                => InitializationContext.game.BoardController.IsEmpty(space);
        }

        public class CardFitsRestriction : SpaceRestrictionElement
        {
            public CardRestriction restriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, IResolutionContext context)
            {
                var card = InitializationContext.game.BoardController.GetCardAt(space);
                return restriction.IsValid(card, context);
            }
        }

        public class SameDiagonal : SpaceRestrictionElement
        {
            public IIdentity<Space> space;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                space.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidSpace(Space space, IResolutionContext context)
                => space.SameDiagonal(this.space.From(context, default));
        }
    }
}