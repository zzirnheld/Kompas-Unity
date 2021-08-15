using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AttackSubeffect : ServerSubeffect
    {
        public int attackerIndex = -2;

        public override Task<ResolutionInfo> Resolve()
        {
            var attacker = Effect.GetTarget(attackerIndex);
            var defender = Target;
            if (attacker == null || defender == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            ServerGame.Attack(attacker, defender, ServerEffect.ServerController);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}