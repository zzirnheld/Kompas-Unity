using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCoordsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.Space == null)
                throw new InvalidSpaceException(Context.Space, NoValidSpaceTarget);

            ServerEffect.AddSpace(Context.Space.Value);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}