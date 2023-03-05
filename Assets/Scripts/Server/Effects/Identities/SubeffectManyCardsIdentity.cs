using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    namespace SubeffectManyCardsIdentities
    {
        public class FittingRestriction : SubeffectIdentityBase<ICollection<GameCardBase>>
        {
            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractItem => InitializationContext.game.Cards
                .Where(card => cardRestriction.IsValidCard(card, InitializationContext.subeffect.CurrentContext))
                .ToArray();
        }

        public class FromActivationContext : SubeffectIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<IReadOnlyCollection<GameCardBase>> cardsFromContext;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardsFromContext.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItem
                => cardsFromContext.From(InitializationContext.subeffect.CurrentContext, default);
        }
    }
}