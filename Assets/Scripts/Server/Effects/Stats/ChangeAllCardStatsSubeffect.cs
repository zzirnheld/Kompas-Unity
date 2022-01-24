using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ChangeAllCardStatsSubeffect : ChangeCardStatsSubeffect
    {
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
            cardRestriction.Initialize(this);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var targets = ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, Context));
            var stats = (NVal, EVal, SVal, WVal, CVal, AVal);
            foreach (var c in targets) c.AddToStats(stats, Effect);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}