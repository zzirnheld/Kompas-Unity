using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class SpaceRestrictionElement : RestrictionElementBase<Space>, IRestrictionElement<GameCardBase>
    {
        public bool IsValid(GameCardBase item, IResolutionContext context) => IsValid(item?.Position, context);
    }

    namespace elements
    {
        public class Not : SpaceRestrictionElement
        {
            public IRestrictionElement<Space> negated;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                negated.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(Space space, IResolutionContext context)
                => !negated.IsValid(space, context);
        }

        public class AnyOf : SpaceRestrictionElement
        {
            public IRestrictionElement<Space>[] restrictions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var r in restrictions) r.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(Space space, IResolutionContext context)
                => restrictions.Any(r => r.IsValid(space, context));
        }

        public class Empty : SpaceRestrictionElement
        {
            protected override bool IsValidLogic(Space space, IResolutionContext context)
                => InitializationContext.game.BoardController.IsEmpty(space);
        }

        public class CardFitsRestriction : SpaceRestrictionElement
        {
            public IRestriction<GameCardBase> restriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(Space space, IResolutionContext context)
            {
                var card = InitializationContext.game.BoardController.GetCardAt(space);
                return restriction.IsValid(card, context);
            }
        }
    }
}