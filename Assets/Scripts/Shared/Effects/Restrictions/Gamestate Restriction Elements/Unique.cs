namespace KompasCore.Effects.Restrictions.GamestateRestrictionElements
{
	public class NoUniqueCopyExists : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context)
			=> !InitializationContext.game.BoardHasCopyOf(InitializationContext.source);
	}
}