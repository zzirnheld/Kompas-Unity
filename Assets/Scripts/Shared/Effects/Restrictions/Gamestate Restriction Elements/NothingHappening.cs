using System.Linq;

namespace KompasCore.Effects.Restrictions.GamestateRestrictionElements
{
	public class NothingHappening : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> InitializationContext.game.NothingHappening;
	}
}