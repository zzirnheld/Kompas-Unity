using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetTurnPlayer : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.playerTargets.Add(Game.TurnPlayer);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
