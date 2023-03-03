using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class ChangeLeyloadSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerGame.Leyload += Count;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}