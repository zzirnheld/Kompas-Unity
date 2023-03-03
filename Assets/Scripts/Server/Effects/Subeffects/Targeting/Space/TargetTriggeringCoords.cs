using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetTriggeringCoords : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CurrentContext.space == null)
                throw new InvalidSpaceException(CurrentContext.space, NoValidSpaceTarget);

            ServerEffect.AddSpace(CurrentContext.space);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}