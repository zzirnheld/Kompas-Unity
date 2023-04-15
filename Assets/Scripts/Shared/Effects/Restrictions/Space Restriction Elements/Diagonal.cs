using System;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    public class SameDiagonal : SpaceRestrictionElement
    {
        //One of these should be non-null. The other one will be replaced by the space to be tested
        public IIdentity<Space> other;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            other.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(Space space, IResolutionContext context)
            => other.From(context, default).SameDiagonal(space);
    }
}