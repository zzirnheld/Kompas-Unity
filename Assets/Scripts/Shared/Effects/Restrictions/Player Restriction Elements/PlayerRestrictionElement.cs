using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class PlayerRestrictionElement : RestrictionElementBase<Player>, IRestriction<GameCardBase>
    {
        public bool IsValid(GameCardBase item, IResolutionContext context)
            => IsValid(item.Controller, context);
    }

    namespace PlayerRestrictionElements
    {
        public class Not : PlayerRestrictionElement
        {
            public IRestriction<Player> negated;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                negated.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(Player item, IResolutionContext context)
                => !negated.IsValid(item, context);
        }
    }
}