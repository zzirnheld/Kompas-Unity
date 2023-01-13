using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class CanMoveTo : CardRestrictionElement
    {
        //public IActivationContextIdentity<Space> contextDestination;
        public INoActivationContextIdentity<Space> destination;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            /*if (contextDestination == null && noContextDestiantion == null)
                throw new System.ArgumentNullException("CanMoveTo has neither a contextual, nor a non-contextual, destination");

            contextDestination?.Initialize(initializationContext);*/
            destination?.Initialize(initializationContext);
        }

        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => card.MovementRestriction.IsValidEffectMove(destination.Item, context);
    }
}