using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DeleteTargetSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (RemoveTarget()) return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
        }
    }
}