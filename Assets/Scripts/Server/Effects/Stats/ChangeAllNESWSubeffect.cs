using KompasCore.Cards;
using KompasCore.Effects;
using System.Linq;

namespace KompasServer.Effects
{
    public class ChangeAllNESWSubeffect : ServerSubeffect
    {
        public int nMult = 0;
        public int eMult = 0;
        public int sMult = 0;
        public int wMult = 0;

        public int nMod = 0;
        public int eMod = 0;
        public int sMod = 0;
        public int wMod = 0;

        public int N => nMult * Effect.X + nMod;
        public int E => eMult * Effect.X + eMod;
        public int S => sMult * Effect.X + sMod;
        public int W => wMult * Effect.X + wMod;

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
            foreach (var c in targets) c.AddToCharStats(N, E, S, W);

            return ServerEffect.ResolveNextSubeffect();
        }
    }
}