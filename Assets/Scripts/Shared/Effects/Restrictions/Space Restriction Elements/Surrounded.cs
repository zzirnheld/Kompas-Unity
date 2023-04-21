namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class Surrounded : SpaceRestrictionElement
	{
		protected override bool IsValidLogic(Space toTest, IResolutionContext context)
			=> InitializationContext.game.BoardController.Surrounded(toTest);
	}
}