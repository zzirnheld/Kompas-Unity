using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DrawSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            var drawn = ServerGame.Draw(Player.Index);
            if (drawn == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}