namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    public class Corner : SpaceRestrictionElement
    {
        protected override bool IsValidLogic(Space toTest, IResolutionContext context)
            => toTest.IsCorner;
    }
}