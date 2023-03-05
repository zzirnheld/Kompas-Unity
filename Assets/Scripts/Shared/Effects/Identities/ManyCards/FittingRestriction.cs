using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManyCards
{
    public class FittingRestriction : ContextualParentIdentityBase<IReadOnlyCollection<GameCardBase>>
    {
        public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new ManyCards.All();

        public CardRestriction cardRestriction;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cards.Initialize(initializationContext);
            cardRestriction.Initialize(initializationContext);
        }

        protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => cards.From(context, secondaryContext).Where(c => cardRestriction.IsValidCard(c, context)).ToArray();
    }
}