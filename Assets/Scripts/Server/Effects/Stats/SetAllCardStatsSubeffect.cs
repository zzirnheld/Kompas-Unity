using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
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
                    CardRestriction.IsCharacter,
                    CardRestriction.Board
                }
            };
            cardRestriction.Initialize(this);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var targets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c, Context));
            foreach (var c in targets)
            {
                c.SetStats(stats: GetRealValues(c), Effect);
            }
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}