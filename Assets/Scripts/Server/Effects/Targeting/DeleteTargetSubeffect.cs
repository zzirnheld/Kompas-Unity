using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DeleteTargetSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            RemoveTarget(); 
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}