using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectManyCardsIdentity : SubeffectInitializeableBase,
        IContextInitializeable, INoActivationContextIdentity<ICollection<GameCardBase>>
    {
        public ICollection<GameCardBase> Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractCards;
            }
        }

        protected abstract ICollection<GameCardBase> AbstractCards { get; }
    }

    namespace SubeffectManyCardsIdentities
    {
        public class FittingRestriction : SubeffectManyCardsIdentity
        {
            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCards => InitializationContext.game.Cards
                .Where(card => cardRestriction.IsValidCard(card, InitializationContext.subeffect.CurrentContext))
                .ToArray();
        }

        public class CardsInPositions : SubeffectManyCardsIdentity
        {
            public INoActivationContextIdentity<ICollection<Space>> positions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
            {
                get
                {
                    var spaces = positions.Item;
                    var cards = spaces.Select(InitializationContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
                    return cards;
                }
            }
        }

        public class FromActivationContext : SubeffectManyCardsIdentity
        {
            public IActivationContextIdentity<ICollection<GameCardBase>> cardsFromContext;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardsFromContext.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
                => cardsFromContext.From(InitializationContext.subeffect.CurrentContext);
        }
    }
}