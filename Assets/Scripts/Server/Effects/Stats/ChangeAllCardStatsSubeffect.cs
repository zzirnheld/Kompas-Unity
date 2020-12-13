using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;

namespace KompasServer.Effects
{
    public class ChangeAllCardStatsSubeffect : ChangeCardStatsSubeffect
    {
        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction = cardRestriction ?? new CardRestriction()
            {
                cardRestrictions = new string[]
                {
                    CardRestriction.IsCharacter,
                    CardRestriction.Board
                }
            };
            cardRestriction.Initialize(this);
        }

        public override bool Resolve()
        {
            var targets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));
            var stats = (NVal, EVal, SVal, WVal, CVal, AVal);
            foreach (var c in targets) c.AddToStats(stats, Effect);

            return ServerEffect.ResolveNextSubeffect();
        }
    }
}