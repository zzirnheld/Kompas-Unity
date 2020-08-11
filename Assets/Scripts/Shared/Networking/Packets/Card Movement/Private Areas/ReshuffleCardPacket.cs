using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class ReshuffleCardPacket : Packet
    {
        public int cardId;
        public int controllerIndex;

        public ReshuffleCardPacket() : base(ReshuffleCard) { }

        public ReshuffleCardPacket(int cardId, int controllerIndex) : this()
        {
            this.cardId = cardId;
            this.controllerIndex = controllerIndex;
        }

        public override Packet Copy() => new ReshuffleCardPacket(cardId, controllerIndex);

        public override Packet GetInversion(bool known)
        {
            if (known) return new DeleteCardPacket(cardId);
            else return null;
        }
    }
}

namespace KompasClient.Networking
{
    public class ReshuffleCardClientPacket : ReshuffleCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var controller = clientGame.ClientPlayers[controllerIndex];
            var card = clientGame.GetCardWithID(cardId);
            if (card != null) card.Reshuffle(controller);
        }
    }
}