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
        public int sourceCardId;

        public AttackStartedPacket() : base(AttackStarted) { }

        public AttackStartedPacket(int sourceCardId) : this()
        {
            this.sourceCardId = sourceCardId;
        }

        public override Packet Copy() => new AttackStartedPacket(sourceCardId);

        public override Packet GetInversion(bool known = true) => new AttackStartedPacket(sourceCardId);
    }
}

namespace KompasClient.Networking
{
    public class AttackStartedClientPacket : EffectActivatedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(sourceCardId);
            if (card != null) card.AttacksThisTurn++;
        }
    }
}