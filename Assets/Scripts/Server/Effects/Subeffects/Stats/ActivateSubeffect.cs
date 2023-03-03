using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class ActivateSubeffect : ServerSubeffect
    {
        public bool activate = true;

        public override Task<ResolutionInfo> Resolve()
        {
            CardTarget.SetActivated(activate, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}