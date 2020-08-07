using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class DeleteCardPacket : Packet
    {
        public int cardId;

        public DeleteCardPacket() : base(DeleteCard) { }

        public DeleteCardPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new DeleteCardPacket(cardId);
    }
}

namespace KompasClient.Networking
{
    public class DeleteCardClientPacket : DeleteCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            clientGame.Delete(card);
        }
    }
}