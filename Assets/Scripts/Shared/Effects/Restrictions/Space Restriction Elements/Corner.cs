namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    /// <summary>
    /// Simplifies the adjacency case, even though it could just be done with "compare distance to 1".
    /// </summary>
    public class Corner : SpaceRestrictionElement
    {
        protected override bool IsValidLogic(Space toTest, IResolutionContext context)
            => toTest.IsCorner;
    }
}