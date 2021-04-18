using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DrawXSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            var drawn = ServerGame.DrawX(Player.index, Count);
            if (drawn.Count < Count) return Task.FromResult(ResolutionInfo.Impossible(CouldntDrawAllX));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}