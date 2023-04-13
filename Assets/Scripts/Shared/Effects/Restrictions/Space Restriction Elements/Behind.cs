using System;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    public class Behind : SpaceRestrictionElement
    {
        //One of these should be non-null. The other one will be replaced by the space to be tested
        public IIdentity<GameCardBase> card;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(Space space, IResolutionContext context)
            => card.From(context, default).SpaceBehind(space);
    }
}