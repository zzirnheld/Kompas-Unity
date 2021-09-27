using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class MoveCardPacket : Packet
    {
        public int cardId;
        public int x;
        public int y;

        public MoveCardPacket() : base(MoveCard) { }

        public MoveCardPacket(int cardId, int x, int y, bool invert) : this()
        {
            this.cardId = cardId;
            this.x = invert ? 6 - x : x;
            this.y = invert ? 6 - y : y;
        }

        public override Packet Copy() => new MoveCardPacket(cardId, x, y, invert: false);

        public override Packet GetInversion(bool known) => new MoveCardPacket(cardId, x, y, invert: true);
    }
}

namespace KompasClient.Networking
{
    public class MoveCardClientPacket : MoveCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if(card != null) card.Move((x, y), false);
            clientGame.uiCtrl.Refresh();
        }
    }
}