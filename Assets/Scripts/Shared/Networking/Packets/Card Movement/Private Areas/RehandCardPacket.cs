using KompasCore.Networking;
using KompasClient.GameCore;
using UnityEngine;

namespace KompasCore.Networking
{
    public class RehandCardPacket : Packet
    {
        public int cardId;

        public RehandCardPacket() : base(RehandCard) { }

        public RehandCardPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new RehandCardPacket(cardId);

        public override Packet GetInversion(bool known)
        {
            if (known) return new RehandCardPacket(cardId);
            else return new ChangeEnemyHandCountPacket(1);
        }
    }
}

namespace KompasClient.Networking
{
    public class RehandCardClientPacket : RehandCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            Debug.Log($"Rehand packet to rehand {cardId}");
            var card = clientGame.GetCardWithID(cardId);
            if (card != null) card.Rehand();
        }
    }
}