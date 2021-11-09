using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class MoveSubeffect : CardChangeStateSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) throw new NullCardException(TargetWasNull);

            Target.Move(Space, false, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}