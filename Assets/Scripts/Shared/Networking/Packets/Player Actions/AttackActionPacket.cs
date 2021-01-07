using KompasCore.Networking;
using KompasServer.GameCore;

namespace KompasCore.Networking
{
    public class AttackActionPacket : Packet
    {
        public int attackerId;
        public int defenderId;

        public AttackActionPacket() : base(AttackAction) { }

        public AttackActionPacket(int attackerId, int defenderId) : this()
        {
            this.attackerId = attackerId;
            this.defenderId = defenderId;
        }

        public override Packet Copy() => new AttackActionPacket(attackerId, defenderId);
    }
}

namespace KompasServer.Networking
{
    public class AttackActionServerPacket : AttackActionPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            var attacker = serverGame.GetCardWithID(attackerId);
            var defender = serverGame.GetCardWithID(defenderId);
            player.TryAttack(attacker, defender);
        }
    }
}