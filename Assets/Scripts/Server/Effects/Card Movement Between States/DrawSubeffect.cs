using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DrawSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerGame.Draw(Player.index);
            if (Target == null) throw new NullCardException(TargetWasNull);
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}