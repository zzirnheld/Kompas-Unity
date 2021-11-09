using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    /// <summary>
    /// Swaps two values among one card's own NESW. E for W, for example.
    /// </summary>
    public class SwapOwnNESWSubeffect : ServerSubeffect
    {
        public int Stat1;
        public int Stat2;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, ChangedStatsOfCardOffBoard);

            int[] newStats = { Target.N, Target.E, Target.S, Target.W };
            (newStats[Stat1], newStats[Stat2]) = (newStats[Stat2], newStats[Stat1]);
            Target.SetCharStats(newStats[0], newStats[1], newStats[2], newStats[3]);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}