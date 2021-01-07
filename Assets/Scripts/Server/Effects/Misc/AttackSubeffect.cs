using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AttackSubeffect : ServerSubeffect
    {
        public int attackerIndex = -2;

        public override async Task<ResolutionInfo> Resolve()
        {
            var attacker = Effect.GetTarget(attackerIndex);
            var defender = Target;
            if (attacker == null || defender == null) return ResolutionInfo.Impossible(TargetWasNull);

            ServerGame.Attack(attacker, defender, ServerEffect.ServerController);
            return ResolutionInfo.Next;
        }
    }
}