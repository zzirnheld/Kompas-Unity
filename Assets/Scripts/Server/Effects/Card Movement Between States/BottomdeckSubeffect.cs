using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class BottomdeckSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) throw new NullCardException(TargetWasNull);

            Target.Bottomdeck(Target.Owner, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}