using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class TargetEnemySubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.playerTargets.Add(EffectController.Enemy);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
