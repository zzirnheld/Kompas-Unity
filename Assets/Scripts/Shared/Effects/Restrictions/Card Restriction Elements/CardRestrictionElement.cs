using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    namespace CardRestrictionElements
    {

        public class Not : NegateRestrictionElementBase<GameCardBase> { }

        public class CardExists : RestrictionElementBase<GameCardBase>
        {
            protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
                => card != null;
        }

        public class Avatar : RestrictionElementBase<GameCardBase>
        {
            protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
                => card.IsAvatar;
        }

        public class Summoned : RestrictionElementBase<GameCardBase>
        {
            protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
                => card.Summoned;
        }
    }
}