using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class HealSubeffect : ServerSubeffect
    {
        public bool forbidNotBoard = true;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            if (forbidNotBoard && Target.Location != CardLocation.Field)
                return Task.FromResult(ResolutionInfo.Impossible(ChangedStatsOfCardOffBoard));
            if (Target.E >= Target.BaseE) return Task.FromResult(ResolutionInfo.Impossible(TooMuchEForHeal));

            Target.SetE(Target.BaseE, stackSrc: ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}