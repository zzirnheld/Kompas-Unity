using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetEnemySubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Effect.players.Add(EffectController.Enemy);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}
