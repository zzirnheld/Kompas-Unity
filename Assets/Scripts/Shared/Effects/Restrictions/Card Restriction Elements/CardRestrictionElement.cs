using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class CardRestrictionElement : RestrictionElementBase<GameCardBase>, IRestriction<Space>
    {
        public bool IsValid(Space item, IResolutionContext context)
            => IsValid(InitializationContext.game.BoardController.GetCardAt(item), context);
    }

    namespace CardRestrictionElements
    {
        public class Not : CardRestrictionElement
        {
            public IRestriction<GameCardBase> negated;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                negated.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
                => !negated.IsValid(item, context);
        }

        public class CardExists : CardRestrictionElement
        {
            protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
                => card != null;
        }
    }
}