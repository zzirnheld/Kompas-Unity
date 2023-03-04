using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.GamestateManyCardsIdentities;
using KompasCore.Exceptions;
using KompasServer.Effects.Identities.SubeffectCardIdentities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Subeffects
{
    public abstract class ChangeCardStatsBase : ServerSubeffect
    {
        public IActivationContextIdentity<GameCardBase> card;
        public IActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cards;

        public IActivationContextIdentity<int> n;
        public IActivationContextIdentity<int> e;
        public IActivationContextIdentity<int> s;
        public IActivationContextIdentity<int> w;
        public IActivationContextIdentity<int> c;
        public IActivationContextIdentity<int> a;

        public IActivationContextIdentity<int> turnsOnBoard;
        public IActivationContextIdentity<int> attacksThisTurn;
        public IActivationContextIdentity<int> spacesMoved;
        public IActivationContextIdentity<int> duration;

        protected IEnumerable<GameCard> cardsToAffect => cards.From(CurrentContext, default).Select(c => c.Card);

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            card ??= new Target() { index = targetIndex };
            cards ??= new Multiple() { cards = new INoActivationContextIdentity<GameCardBase>[] { card } };

            var initContext = DefaultInitializationContext;
            cards.Initialize(initContext);

            n?.Initialize(initContext);
            e?.Initialize(initContext);
            s?.Initialize(initContext);
            w?.Initialize(initContext);
            c?.Initialize(initContext);
            a?.Initialize(initContext);

            turnsOnBoard?.Initialize(initContext);
            attacksThisTurn?.Initialize(initContext);
            spacesMoved?.Initialize(initContext);
            duration?.Initialize(initContext);
        }

        protected void ValidateCardOnBoard(GameCard card)
        {
            if (forbidNotBoard && card.Location != CardLocation.Board)
                throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);
        }
    }
}