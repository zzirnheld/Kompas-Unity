using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class PayStatsSubeffect : ServerSubeffect
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

        public override bool IsImpossible() => Target == null || Target.N < N || Target.E < E || Target.S < S || Target.W < W;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            if (Target.N < N ||
                Target.E < E ||
                Target.S < S ||
                Target.W < W)
                return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

            Target.AddToCharStats(-1 * N, -1 * E, -1 * S, -1 * W, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}