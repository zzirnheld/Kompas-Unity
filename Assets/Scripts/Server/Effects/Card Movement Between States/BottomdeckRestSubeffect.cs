using KompasCore.Cards;
using KompasCore.Cards.Movement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class BottomdeckRestSubeffect : ServerSubeffect
    {
        private static readonly System.Random rng = new System.Random();

        private List<GameCard> Shuffle(List<GameCard> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }

        public override Task<ResolutionInfo> Resolve()
        {
            //TODO better shuffling algorithm
            var list = Shuffle(Effect.rest);
            foreach (GameCard c in list) c.Bottomdeck(c.Owner, Effect);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}