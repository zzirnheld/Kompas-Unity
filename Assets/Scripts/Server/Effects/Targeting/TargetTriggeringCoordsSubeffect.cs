using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCoordsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.space == null)
                throw new InvalidSpaceException(Context.space, NoValidSpaceTarget);

            ServerEffect.AddSpace(Context.space);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}