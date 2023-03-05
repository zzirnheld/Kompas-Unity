using KompasCore.Cards;
using KompasCore.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManyCardsIdentities
    {
        public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            protected override IReadOnlyCollection<GameCardBase> AbstractItem => InitializationContext.game.Cards;
        }

        public class FittingRestriction : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new All();

            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => cards.From(context, secondaryContext)
                        .Where(c => cardRestriction.IsValidCard(c, default))
                        .ToArray();
        }

        public class Multiple : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<GameCardBase>[] cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var i in cards) i.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => cards.Select(s => s.From(context, secondaryContext)).ToArray();
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
                var cards = spaces.Select(InitializationContext.game.BoardController.GetCardAt)
                    .Where(s => s != null)
                    .ToArray();
                return cards;
            }
        }

        public class Board : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            protected override IReadOnlyCollection<GameCardBase> AbstractItem
                => InitializationContext.game.BoardController.Cards.ToArray();
        }

        public class Discard : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public bool friendly = true;
            public bool enemy = true;

            protected override IReadOnlyCollection<GameCardBase> AbstractItem
            {
                get
                {
                    var cards = new List<GameCardBase>();
                    if (friendly) cards.AddRange(InitializationContext.Controller.discardCtrl.Cards);
                    if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.discardCtrl.Cards);
                    return cards;
                }
            }
        }

        public class Deck : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public bool friendly = true;
            public bool enemy = true;

            protected override IReadOnlyCollection<GameCardBase> AbstractItem
            {
                get
                {
                    var cards = new List<GameCardBase>();
                    if (friendly) cards.AddRange(InitializationContext.Controller.deckCtrl.Deck);
                    if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.deckCtrl.Deck);
                    return cards;
                }
            }
        }

        public class Limit : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<int> limit;
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                limit.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => CollectionsHelper.Shuffle(cards.From(context, secondaryContext))
                    .Take(limit.From(context, secondaryContext))
                    .ToArray();
        }

        public class Distinct : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => cards.From(context, secondaryContext)
                    .GroupBy(c => c.CardName)
                    .Select(group => group.First())
                    .ToArray();
        }
    }
}