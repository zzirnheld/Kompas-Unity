namespace KompasCore.Effects.Restrictions.elements
{
    public class Edge : SpaceRestrictionElement
    {
        protected override bool IsValidLogic(Space toTest, IResolutionContext context)
            => toTest.IsEdge;
    }
}