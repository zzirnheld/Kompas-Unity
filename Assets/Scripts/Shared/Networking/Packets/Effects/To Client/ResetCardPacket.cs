using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class ResetCardPacket : Packet
    {
        public int cardId;

        public ResetCardPacket() : base(ResetCard) { }

        public ResetCardPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new ResetCardPacket(cardId);
    }
}

namespace KompasClient.Networking
{
    public class ResetCardClientPacket : ResetCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if (card != null) card.ResetCard();
        }
    }
}