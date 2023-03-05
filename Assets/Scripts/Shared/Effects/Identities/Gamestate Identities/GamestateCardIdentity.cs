using KompasCore.Cards;
using KompasCore.Effects.Selectors;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateCardIdentities
    {
        public class SelectFromMany : ContextualIdentityBase<GameCardBase>
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

        public class AugmentedCard : ContextualIdentityBase<GameCardBase>
        {
            public IIdentity<GameCardBase> ofThisCard;
            
            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                ofThisCard.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => ofThisCard.From(context, secondaryContext).AugmentedCard;
        }

        public class Avatar : ContextualIdentityBase<GameCardBase>
        {
            public IIdentity<Player> player;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                player.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => player.From(context, secondaryContext).Avatar;
        }
    }
}