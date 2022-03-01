using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class UpdateKnownToEnemyPacket : Packet
    {
        public bool known;
        public int id;

        public UpdateKnownToEnemyPacket() : base(KnownToEnemy) { }

        public UpdateKnownToEnemyPacket(bool known, int id) : this()
        {
            this.known = known;
            this.id = id;
        }

        public override Packet Copy() => new UpdateKnownToEnemyPacket(known, id);
    }
}

namespace KompasClient.Networking
{
    public class UpdateKnownToEnemyClientPacket : UpdateKnownToEnemyPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(id);
            if (card == null) throw new System.ArgumentException($"Couldn't find card with id {id} to set known to {known}");
            card.KnownToEnemy = known;
        }
    }
}