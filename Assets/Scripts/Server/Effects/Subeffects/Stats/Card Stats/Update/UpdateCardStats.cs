using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.GamestateManyCardsIdentities;
using KompasCore.Effects.Identities.GamestateNumberIdentities;
using KompasServer.Effects.Identities.SubeffectCardIdentities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class UpdateCardStats : ServerSubeffect
    {
        public IActivationContextIdentity<GameCardBase> card = new Target();
        public IActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cards;

        // Standard card stats. Never null.
        public IActivationContextIdentity<int> nChange = new Constant() { constant = 0 };
        public IActivationContextIdentity<int> eChange = new Constant() { constant = 0 };
        public IActivationContextIdentity<int> sChange = new Constant() { constant = 0 };
        public IActivationContextIdentity<int> wChange = new Constant() { constant = 0 };
        public IActivationContextIdentity<int> cChange = new Constant() { constant = 0 };
        public IActivationContextIdentity<int> aChange = new Constant() { constant = 0 };

        public IActivationContextIdentity<int> turnsOnBoardChange;
        public IActivationContextIdentity<int> attacksThisTurnChange;
        public IActivationContextIdentity<int> spacesMovedChange;
        public IActivationContextIdentity<int> durationChange;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            card ??= new Target();
            cards ??= new Multiple() { cards = new INoActivationContextIdentity<GameCardBase>[] { card } };

            var initContext = DefaultInitializationContext;
            cards.Initialize(initContext);

            nChange.Initialize(initContext);
            eChange.Initialize(initContext);
            sChange.Initialize(initContext);
            wChange.Initialize(initContext);
            cChange.Initialize(initContext);
            aChange.Initialize(initContext);

            turnsOnBoardChange?.Initialize(initContext);
            attacksThisTurnChange?.Initialize(initContext);
            spacesMovedChange?.Initialize(initContext);
            durationChange?.Initialize(initContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            int nChange = this.nChange.From(CurrentContext, default);
            int eChange = this.eChange.From(CurrentContext, default);
            int sChange = this.sChange.From(CurrentContext, default);
            int wChange = this.wChange.From(CurrentContext, default);
            int cChange = this.cChange.From(CurrentContext, default);
            int aChange = this.aChange.From(CurrentContext, default);

            int? turnsOnBoardChange = this.turnsOnBoardChange?.From(CurrentContext, default);
            int? attacksThisTurnChange = this.attacksThisTurnChange?.From(CurrentContext, default);
            int? spacesMovedChange = this.spacesMovedChange?.From(CurrentContext, default);
            int? durationChange = this.durationChange?.From(CurrentContext, default);

            foreach (var card in cards.From(CurrentContext, default).Select(c => c.Card))
            {
                card.AddToStats((nChange, eChange, sChange, wChange, cChange, aChange), Effect);

                if (turnsOnBoardChange.HasValue) card.TurnsOnBoard += turnsOnBoardChange.Value;
                if (attacksThisTurnChange.HasValue) card.AttacksThisTurn += attacksThisTurnChange.Value;
                if (spacesMovedChange.HasValue) card.SpacesMoved += spacesMovedChange.Value;
                if (durationChange.HasValue) card.Duration += durationChange.Value;
            }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}