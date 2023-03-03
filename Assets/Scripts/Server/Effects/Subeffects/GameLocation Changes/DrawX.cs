using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class DrawX : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            var drawn = ServerGame.DrawX(PlayerTarget, Count, Effect);
            if (drawn.Count < Count) return Task.FromResult(ResolutionInfo.Impossible(CouldntDrawAllX));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}