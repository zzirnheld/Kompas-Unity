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

        public PlayCardPacket(int cardId, string cardName, int controllerIndex, int x, int y, bool invert = false) : this()
        {
            this.cardId = cardId;
            this.cardName = cardName;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
            this.x = invert ? 6 - x : x;
            this.y = invert ? 6 - y : y;
        }

        public override Packet Copy() => new PlayCardPacket(cardId, cardName, controllerIndex, x, y, invert: false);

        public override Packet GetInversion(bool known)
        {
            if (known) return new PlayCardPacket(cardId, cardName, controllerIndex, x, y, invert: true);
            else return new AddCardPacket(cardId, cardName, CardLocation.Field, controllerIndex, x, y, false, invert: true);
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