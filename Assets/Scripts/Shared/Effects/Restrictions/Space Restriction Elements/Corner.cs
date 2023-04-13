namespace KompasCore.Effects.Restrictions.elements
{
    public class Corner : SpaceRestrictionElement
    {
        protected override bool IsValidLogic(Space toTest, IResolutionContext context)
            => toTest.IsCorner;
    }
}