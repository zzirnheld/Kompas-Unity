using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{

    namespace SubeffectManyCardsIdentities
    {
        public class FromActivationContext : SubeffectIdentityBase<ICollection<GameCardBase>>
        {
            public IActivationContextIdentity<ICollection<GameCardBase>> cardsFromContext;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardsFromContext.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractItem
                => cardsFromContext.From(InitializationContext.subeffect.CurrentContext, default);
        }

        public class FittingRestriction : SubeffectIdentityBase<ICollection<GameCardBase>>
        {
            public INoActivationContextIdentity<ICollection<GameCardBase>> cards;
            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards?.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractItem
            {
                get
                {
                    //Making this game.cards ?? cards.item is only valid in C# 9+
                    if (cards != null) return cards.Item
                       .Where(card => cardRestriction.IsValidCard(card, InitializationContext.subeffect.CurrentContext))
                       .ToArray();
                    else return InitializationContext.game.Cards
                       .Where(card => cardRestriction.IsValidCard(card, InitializationContext.subeffect.CurrentContext))
                       .ToArray();
                }
            }
        }

        public class CardsInPositions : SubeffectIdentityBase<ICollection<GameCardBase>>
        {
            public INoActivationContextIdentity<ICollection<Space>> positions;
            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
                cardRestriction?.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractItem
            {
                get
                {
                    var spaces = positions.Item;
                    var cardsEnumerable = spaces.Select(InitializationContext.game.boardCtrl.GetCardAt).Where(s => s != null);
                    if (cardRestriction != null) cardsEnumerable = cardsEnumerable.Where(c => cardRestriction.IsValidCard(c, default));
                    return cardsEnumerable.ToArray();
                }
            }
        }

        public class CardsInAOE : SubeffectIdentityBase<ICollection<GameCardBase>>
        {
            public INoActivationContextIdentity<GameCardBase> whoseAOE;
            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                whoseAOE.Initialize(initializationContext);
                cardRestriction?.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractItem
            {
                get
                {
                    var cardsEnumerable = InitializationContext.game.Cards.Where(whoseAOE.Item.IsCardInMyAOE);
                    if (cardRestriction != null) cardsEnumerable = cardsEnumerable.Where(c => cardRestriction.IsValidCard(c, default));
                    return cardsEnumerable.ToArray();
                }
            }

        }
    }
}