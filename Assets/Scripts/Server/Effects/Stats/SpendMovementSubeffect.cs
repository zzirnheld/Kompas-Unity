using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SpendMovementSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() => Target == null || Target.SpacesCanMove < Count;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (Target.SpacesCanMove < Count)
                return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

            Target.SetSpacesMoved(Target.SpacesMoved + Count);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}