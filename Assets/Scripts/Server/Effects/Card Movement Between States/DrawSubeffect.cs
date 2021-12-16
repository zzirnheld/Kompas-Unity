using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DrawSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerGame.Draw(Player.index);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}