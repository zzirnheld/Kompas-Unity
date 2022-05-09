using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectManyCardsIdentity : ContextInitializeableBase, IContextInitializeable
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

    public interface ISubeffectManyCardsIdentity : IContextInitializeable
    {
        public ICollection<GameCardBase> Cards { get; }
    }

    namespace SubeffectManyCardsIdentities
    {
        public class FittingRestriction : SubeffectManyCardsIdentity
        {
            public CardRestriction cardRestriction;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardRestriction.Initialize(source: restrictionContext.source, restrictionContext.subeffect?.Effect, restrictionContext.subeffect);
            }

            protected override ICollection<GameCardBase> AbstractCards => RestrictionContext.game.Cards
                .Where(card => cardRestriction.IsValidCard(card, RestrictionContext.subeffect.CurrentContext))
                .ToArray();
        }

        public class CardsInPositions : SubeffectManyCardsIdentity
        {
            public SubeffectManySpacesIdentity positions;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                positions.Initialize(restrictionContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
            {
                get
                {
                    var spaces = positions.Spaces;
                    var cards = spaces.Select(RestrictionContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
                    return cards;
                }
            }
        }

        public class FromActivationContext : SubeffectManyCardsIdentity
        {
            public IActivationContextManyCardsIdentity cardsFromContext;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardsFromContext.Initialize(restrictionContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
                => cardsFromContext.CardsFrom(RestrictionContext.subeffect.CurrentContext);
        }
    }
}