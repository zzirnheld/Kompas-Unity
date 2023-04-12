using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class CardsMatch : CardRestrictionElement
    {
        public IIdentity<GameCardBase> card;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
         => item?.Card == card.From(context, default).Card;
    }
}