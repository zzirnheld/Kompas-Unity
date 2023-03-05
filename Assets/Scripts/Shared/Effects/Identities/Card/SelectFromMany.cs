using KompasCore.Cards;
using KompasCore.Effects.Selectors;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities.Cards
{
    public class SelectFromMany : ContextualParentIdentityBase<GameCardBase>
    {
        public ISelector<GameCardBase> selector = new RandomCard();
        public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new GamestateManyCardsIdentities.All();

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cards.Initialize(initializationContext);
        }

        protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => selector.Select(cards.From(context, secondaryContext));
    }
}