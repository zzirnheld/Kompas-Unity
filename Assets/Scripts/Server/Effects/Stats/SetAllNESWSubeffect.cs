using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.Cards;
using System.Linq;

namespace KompasServer.Effects
{
    public class SetAllNESWSubeffect : SetNESWSubeffect
    {
        private (int, int, int, int) GetRealValues(GameCard c)
        {
            (int n, int e, int s, int w) = (
                nVal >= 0 ? nVal : c.N,
                eVal >= 0 ? eVal : c.E,
                sVal >= 0 ? sVal : c.S,
                wVal >= 0 ? wVal : c.W
            );
            return (n, e, s, w);
        }

        //default to making sure things are characters before changing their stats
        public CardRestriction cardRestriction = new CardRestriction()
        {
            cardRestrictions = new string[]
            {
            CardRestriction.IsCharacter,
            CardRestriction.Board
            }
        };

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
        }

        public override bool Resolve()
        {
            var targets = ServerGame.Cards.Where(c => cardRestriction.Evaluate(c));
            foreach (ServerGameCard c in targets)
            {
                var (n, e, s, w) = GetRealValues(c);
                c.SetN(n, ServerEffect);
                c.SetE(e, ServerEffect);
                c.SetS(s, ServerEffect);
                c.SetW(w, ServerEffect);
            }
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}