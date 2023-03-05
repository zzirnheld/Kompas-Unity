using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace ActivationContextManyCardsIdentities
    {
        public class CardsInPositions : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<IReadOnlyCollection<Space>> positions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            {
                var spaces = positions.From(context, secondaryContext);
                return spaces.Select(InitializationContext.game.BoardController.GetCardAt).Where(s => s != null).ToArray();
            }
        }

        public class Augments : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<GameCardBase> card;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                card.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => card.From(context, secondaryContext).Augments;
        }

        public class FittingRestriction : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new GamestateManyCardsIdentities.All();

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
}