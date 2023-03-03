using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class DrawSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerGame.Draw(PlayerTarget);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}