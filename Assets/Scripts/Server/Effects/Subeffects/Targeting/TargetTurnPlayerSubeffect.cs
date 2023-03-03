using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetTurnPlayerSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.playerTargets.Add(Game.TurnPlayer);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
