using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class SetAllCardStatsSubeffect : SetCardStatsSubeffect
    {
        private (int, int, int, int, int, int) GetRealValues(GameCard card)
        {
            (int n, int e, int s, int w, int c, int a) = (
                nVal >= 0 ? nVal : card.N,
                eVal >= 0 ? eVal : card.E,
                sVal >= 0 ? sVal : card.S,
                wVal >= 0 ? wVal : card.W,
                cVal >= 0 ? cVal : card.C,
                aVal >= 0 ? aVal : card.A
            );
            return (n, e, s, w, c, a);
        }

        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction()
            {
                cardRestrictions = new string[]
                {
                    CardRestriction.Character,
                    CardRestriction.Board
                }
            };
            cardRestriction.Initialize(DefaultInitializationContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var targets = ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, CurrentContext));
            foreach (var card in targets)
            {
                if (forbidNotBoard && card.Location != CardLocation.Board)
                    throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);
            }
            foreach (var c in targets)
            {
                c.SetStats(stats: GetRealValues(c), Effect);
            }
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}