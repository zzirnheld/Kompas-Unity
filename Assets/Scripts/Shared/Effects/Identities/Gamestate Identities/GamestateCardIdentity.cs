using KompasCore.Cards;
using KompasCore.Effects.Selectors;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateCardIdentities
    {
        public class SelectFromMany : NoActivationContextIdentityBase<GameCardBase>
        {
            public ISelector<GameCardBase> selector = new RandomCard();
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cards = new GamestateManyCardsIdentities.All();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItem => selector.Select(cards.Item);
        }

        public class ThisCard : NoActivationContextIdentityBase<GameCardBase>
        {
            protected override GameCardBase AbstractItem => InitializationContext.source;
        }

        public class AugmentedCard : NoActivationContextIdentityBase<GameCardBase>
        {
            public INoActivationContextIdentity<GameCardBase> ofThisCard;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                ofThisCard.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItem
                => ofThisCard.Item.AugmentedCard;
        }

        public class Avatar : NoActivationContextIdentityBase<GameCardBase>
        {
            public INoActivationContextIdentity<Player> player;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                player.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItem => player.Item.Avatar;
        }
    }
}