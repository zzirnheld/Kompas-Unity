using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ActivateSubeffect : ServerSubeffect
    {
        public override async Task<ResolutionInfo> Resolve()
        {
            Target.SetActivated(true, Effect);
            return ResolutionInfo.Next;
        }
    }
}