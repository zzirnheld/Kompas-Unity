using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ActivateSubeffect : ServerSubeffect
    {
        public bool activate = true;

        public override Task<ResolutionInfo> Resolve()
        {
            Target.SetActivated(activate, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}