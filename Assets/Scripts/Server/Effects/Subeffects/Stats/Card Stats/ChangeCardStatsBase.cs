using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.ActivationContextCardIdentities;
using KompasCore.Effects.Identities.GamestateManyCardsIdentities;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Subeffects
{
    public abstract class ChangeCardStatsBase : ServerSubeffect
    {
        public IIdentity<GameCardBase> card;
        public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

        public IIdentity<int> n;
        public IIdentity<int> e;
        public IIdentity<int> s;
        public IIdentity<int> w;
        public IIdentity<int> c;
        public IIdentity<int> a;

        public IIdentity<int> turnsOnBoard;
        public IIdentity<int> attacksThisTurn;
        public IIdentity<int> spacesMoved;
        public IIdentity<int> duration;

        protected IEnumerable<GameCard> cardsToAffect => cards.From(CurrentContext, default).Select(c => c.Card);

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            card ??= new TargetIndex() { index = targetIndex };
            cards ??= new Multiple() { cards = new IIdentity<GameCardBase>[] { card } };

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