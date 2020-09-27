using System.Linq;

namespace KompasServer.Effects
{
    public class AttackSubeffect : ServerSubeffect
    {
        public int attackerIndex = -2;

        public override bool Resolve()
        {
            var attacker = Effect.GetTarget(attackerIndex);
            var defender = Target;
            if (attacker == null || defender == null) ServerEffect.EffectImpossible();

            ServerGame.Attack(attacker, defender, ServerEffect.ServerController);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}