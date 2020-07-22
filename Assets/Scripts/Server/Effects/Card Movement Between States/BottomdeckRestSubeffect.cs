using KompasCore.Cards;
using System.Collections.Generic;

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
                GameCard value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public override bool Resolve()
        {
            //TODO better shuffling algorithm
            var list = Shuffle(Effect.Rest);
            foreach (GameCard c in list) c.Bottomdeck(c.Owner, Effect);

            return ServerEffect.ResolveNextSubeffect();
        }
    }
}