using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectManyCardsIdentity : SubeffectInitializeableBase,
        IContextInitializeable
    {
        public ICollection<GameCardBase> Cards
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
            public INoActivationContextManySpacesIdentity positions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
            {
                get
                {
                    var spaces = positions.Spaces;
                    var cards = spaces.Select(InitializationContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
                    return cards;
                }
            }
        }

        public class FromActivationContext : SubeffectManyCardsIdentity
        {
            public IActivationContextManyCardsIdentity cardsFromContext;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardsFromContext.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
                => cardsFromContext.CardsFrom(InitializationContext.subeffect.CurrentContext);
        }
    }
}