using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTriggeringCoordsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.Space == null)
                return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

            ServerEffect.AddSpace(Context.Space.Value);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}