using KompasCore.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class SetCardStats : ChangeCardStatsBase
    {

        public override Task<ResolutionInfo> Resolve()
        {
            int? nValue = n?.From(CurrentContext, default);
            int? eValue = e?.From(CurrentContext, default);
            int? sValue = s?.From(CurrentContext, default);
            int? wValue = w?.From(CurrentContext, default);
            int? cValue = c?.From(CurrentContext, default);
            int? aValue = a?.From(CurrentContext, default);

            int? turnsOnBoardChange     = turnsOnBoard?.From(CurrentContext, default);
            int? attacksThisTurnChange  = attacksThisTurn?.From(CurrentContext, default);
            int? spacesMovedChange      = spacesMoved?.From(CurrentContext, default);
            int? durationChange         = duration?.From(CurrentContext, default);

            foreach (var card in cards.From(CurrentContext, default).Select(c => c.Card))
            {
                ValidateCardOnBoard(card);

                card.SetStats(card.Stats.ReplaceWith((nValue, eValue, sValue, wValue, cValue, aValue)), Effect);

                if (turnsOnBoardChange.HasValue)    card.TurnsOnBoard       = turnsOnBoardChange.Value;
                if (attacksThisTurnChange.HasValue) card.AttacksThisTurn    = attacksThisTurnChange.Value;
                if (spacesMovedChange.HasValue)     card.SpacesMoved        = spacesMovedChange.Value;
                if (durationChange.HasValue)        card.Duration           = durationChange.Value;
            }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}