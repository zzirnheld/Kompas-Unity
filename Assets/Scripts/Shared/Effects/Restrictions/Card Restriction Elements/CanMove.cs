using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class CanMove : CardRestrictionElement
    {
        public IIdentity<Space> destination;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            destination?.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => card.MovementRestriction.IsValidEffectMove(destination.From(context, default), context);
    }
}