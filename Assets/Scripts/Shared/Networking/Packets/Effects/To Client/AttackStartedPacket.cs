using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class AttackStartedPacket : Packet
    {
        public int attackerId;
        public int defenderId;

        public AttackStartedPacket() : base(AttackStarted) { }

        public AttackStartedPacket(int attackerId, int defenderId) : this()
        {
            this.attackerId = attackerId;
            this.defenderId = defenderId;
        }

        public override Packet Copy() => new AttackStartedPacket(attackerId, defenderId);
    }
}

namespace KompasClient.Networking
{
    public class AttackStartedClientPacket : AttackStartedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var attacker = clientGame.GetCardWithID(attackerId);
            var defender = clientGame.GetCardWithID(defenderId);
            //if (card != null) card.AttacksThisTurn++;
            //don't do this because AttacksThisTurn should be updated by server and told to client
        }
    }
}