using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
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