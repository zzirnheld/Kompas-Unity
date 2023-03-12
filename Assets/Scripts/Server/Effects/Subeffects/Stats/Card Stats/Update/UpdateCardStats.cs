using System.Threading.Tasks;
using KompasCore.Effects.Identities.Numbers;

namespace KompasServer.Effects.Subeffects
{
    public class UpdateCardStats : ChangeCardStatsBase
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            n ??= new Constant() { constant = 0 };
            e ??= new Constant() { constant = 0 };
            s ??= new Constant() { constant = 0 };
            w ??= new Constant() { constant = 0 };
            c ??= new Constant() { constant = 0 };
            a ??= new Constant() { constant = 0 };
            base.Initialize(eff, subeffIndex);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            int nChange = n.From(CurrentContext, default);
            int eChange = e.From(CurrentContext, default);
            int sChange = s.From(CurrentContext, default);
            int wChange = w.From(CurrentContext, default);
            int cChange = c.From(CurrentContext, default);
            int aChange = a.From(CurrentContext, default);

            int? turnsOnBoardChange     = turnsOnBoard?.From(CurrentContext, default);
            int? attacksThisTurnChange  = attacksThisTurn?.From(CurrentContext, default);
            int? spacesMovedChange      = spacesMoved?.From(CurrentContext, default);
            int? durationChange         = duration?.From(CurrentContext, default);

            foreach (var card in cardsToAffect)
            {
                ValidateCardOnBoard(card);

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