using System.Linq;

namespace KompasCore.Effects.Restrictions.GamestateRestrictionElements
{
    public class EffectAlreadyTriggered : GamestateRestrictionBase
    {
        protected override bool IsValidLogic(IResolutionContext context)
            => InitializationContext.game.StackEntries.Any(e => e == InitializationContext.effect);
    }
}