using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class JumpSubeffect : ServerSubeffect
    {
        public int indexToJumpTo;

        public override Task<ResolutionInfo> Resolve()
        {
            //this will always jump to the given subeffect index
            return Task.FromResult(ResolutionInfo.Index(indexToJumpTo));
        }
    }
}