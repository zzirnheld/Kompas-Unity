using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class CanPlay : CardRestrictionElement
    {
        public IIdentity<Space> destination;
        public string[] ignoring = { };

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            destination?.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
        {
            if (destination == null) return Space.Spaces.Any(s => card.PlayRestriction.IsValidEffectPlay(s, context));
            else return card.PlayRestriction.IsValidEffectPlay(destination.From(context, default), context);
        }
    }
}