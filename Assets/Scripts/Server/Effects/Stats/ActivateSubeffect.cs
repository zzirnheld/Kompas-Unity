using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ActivateSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Target.SetActivated(true, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}