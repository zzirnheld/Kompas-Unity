using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;
using System.Diagnostics;

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
            var card = clientGame.GetCardWithID(cardId);
            UnityEngine.Debug.Log($"Rehand packet to rehand {card?.CardName} that has controller {card?.ControllerIndex}");
            if (card != null) card.Rehand();
        }
    }
}