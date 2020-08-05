using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class PlayCardPacket : Packet
    {
        public int cardId;
        public string cardName;
        public int controllerIndex;
        public int x;
        public int y;

        public PlayCardPacket() : base(PlayCard) { }

        public PlayCardPacket(int cardId, string cardName, int controllerIndex, int x, int y) : this()
        {
            this.cardId = cardId;
            this.cardName = cardName;
            this.controllerIndex = controllerIndex;
            this.x = controllerIndex == 0 ? x : 6 - x;
            this.y = controllerIndex == 0 ? y : 6 - y;
        }

        public override Packet Copy() => new PlayCardPacket(cardId, cardName, controllerIndex, x, y);

        public override Packet GetInversion(bool known)
        {
            if (known) return new PlayCardPacket(cardId, cardName, 1 - controllerIndex, x, y);
            else return new AddCardPacket(cardId, cardName, CardLocation.Field, controllerIndex, x, y, false);
        }
    }
}

namespace KompasClient.Networking
{
    public class PlayCardClientPacket : PlayCardPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var controller = clientGame.ClientPlayers[controllerIndex];
            var card = clientGame.GetCardWithID(cardId);
            card.Play(x, y, controller);
        }
    }
}