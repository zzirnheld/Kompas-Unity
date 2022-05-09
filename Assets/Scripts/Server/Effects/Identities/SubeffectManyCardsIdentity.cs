using KompasCore.Cards;
using KompasCore.Effects;
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

    namespace SubeffectManyCardsIdentities
    {
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
            public ActivationContextCardsIdentity cardsIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardsIdentity.Initialize(restrictionContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
                => cardsIdentity.CardsFrom(RestrictionContext.subeffect.Context);
        }
    }
}