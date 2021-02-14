using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetEnemySubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.players.Add(EffectController.Enemy);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
