using KompasCore.Cards;
using KompasCore.Helpers;
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
                => cards.Item.Where(c => cardRestriction.IsValidCard(c, default)).ToArray();
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

        public class Discard : NoActivationContextIdentityBase<IReadOnlyCollection<GameCardBase>>
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

        public class Limit : NoActivationContextIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public INoActivationContextIdentity<int> limit;
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                limit.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItem
                => CollectionsHelper.Shuffle(cards.Item).Take(limit.Item).ToArray();
        }
    }
}