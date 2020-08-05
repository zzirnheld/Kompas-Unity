using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class MoveCardPacket : Packet
    {
        public int cardId;
        public int x;
        public int y;
        public bool normalMove;

        public MoveCardPacket() : base(MoveCard) { }

        public MoveCardPacket(int cardId, int x, int y, bool normalMove, bool invert) : this()
        {
            this.cardId = cardId;
            this.x = invert ? x : 6 - x;
            this.y = invert ? y : 6 - y;
            this.normalMove = normalMove;
        }

        public override Packet Copy() => new MoveCardPacket(cardId, x, y, normalMove, invert: false);

        public override Packet GetInversion(bool known) => new MoveCardPacket(cardId, x, y, normalMove, invert: true);
    }
}

namespace KompasClient.Networking
{
    public class MoveCardClientPacket : MoveCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if(card != null) card.Move(x, y, normalMove);
            clientGame.uiCtrl.RefreshShownCardInfo();
        }
    }
}