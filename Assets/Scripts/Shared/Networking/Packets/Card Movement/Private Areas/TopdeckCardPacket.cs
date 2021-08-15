using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class TopdeckCardPacket : Packet
    {
        public int cardId;
        public int controllerIndex;

        public TopdeckCardPacket() : base(TopdeckCard) { }

        public TopdeckCardPacket(int cardId, int controllerIndex, bool invert = false) : this()
        {
            this.cardId = cardId;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
        }

        public override Packet Copy() => new TopdeckCardPacket(cardId, controllerIndex);

        public override Packet GetInversion(bool known)
        {
            if (known) return new DeleteCardPacket(cardId);
            else return null;
        }
    }
}

namespace KompasClient.Networking
{
    public class TopdeckCardClientPacket : TopdeckCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var controller = clientGame.ClientPlayers[controllerIndex];
            var card = clientGame.GetCardWithID(cardId);
            if(card != null) card.Topdeck(controller);
        }
    }
}