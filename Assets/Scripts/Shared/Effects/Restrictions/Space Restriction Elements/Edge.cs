namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class Edge : SpaceRestrictionElement
	{
		protected override bool IsValidLogic(Space toTest, IResolutionContext context)
			=> toTest.IsEdge;
	}
}