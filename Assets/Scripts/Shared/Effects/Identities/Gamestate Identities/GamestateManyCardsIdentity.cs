using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManyCardsIdentities
    {
        public class All : NoActivationContextIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            protected override IReadOnlyCollection<GameCardBase> AbstractItem => InitializationContext.game.Cards;
        }

        public class FittingRestriction : NoActivationContextIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cards = new All();

            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItem
                => InitializationContext.game.Cards.Where(c => cardRestriction.IsValidCard(c, default)).ToArray();
        }

        public class Multiple : NoActivationContextIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public INoActivationContextIdentity<GameCardBase>[] cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var i in cards) i.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItem => cards.Select(s => s.Item).ToArray();
        }

        public class CardsInPositions : NoActivationContextIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public INoActivationContextIdentity<IReadOnlyCollection<Space>> positions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItem
            {
                get
                {
                    var spaces = positions.Item;
                    var cards = spaces.Select(InitializationContext.game.BoardController.GetCardAt)
                        .Where(s => s != null)
                        .ToArray();
                    return cards;
                }
            }
        }

        public class Board : NoActivationContextIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            protected override IReadOnlyCollection<GameCardBase> AbstractItem
                => InitializationContext.game.BoardController.Cards.ToArray();
        }
    }
}