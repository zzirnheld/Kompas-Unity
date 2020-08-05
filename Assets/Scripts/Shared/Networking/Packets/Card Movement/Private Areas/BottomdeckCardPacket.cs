using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class BottomdeckCardPacket : Packet
    {
        public int cardId;
        public int controllerIndex;

        public BottomdeckCardPacket() : base(BottomdeckCard) { }

        public BottomdeckCardPacket(int cardId, int controllerIndex) : this()
        {
            this.cardId = cardId;
            this.controllerIndex = controllerIndex;
        }

        public override Packet Copy() => new BottomdeckCardPacket(cardId, controllerIndex);

        public override Packet GetInversion(bool known)
        {
            if (known) return new DeleteCardPacket(cardId);
            else return null;
        }
    }
}

namespace KompasClient.Networking
{
    public class BottomdeckCardClientPacket : BottomdeckCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var controller = clientGame.ClientPlayers[controllerIndex];
            var card = clientGame.GetCardWithID(cardId);
            if (card != null) card.Bottomdeck(controller);
        }
    }
}