using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class HealSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, ChangedStatsOfCardOffBoard);
            else if (Target.E >= Target.BaseE) 
                throw new InvalidCardException(Target, TooMuchEForHeal);

            Target.SetE(Target.BaseE, stackSrc: ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}