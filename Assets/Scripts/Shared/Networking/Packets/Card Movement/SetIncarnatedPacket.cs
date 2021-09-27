using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class SetIncarnatedPacket : Packet
    {
        public int cardId;
        public bool incarnated;

        public SetIncarnatedPacket() : base(Incarnate) { }

        public SetIncarnatedPacket(int cardId, bool incarnated) : this()
        {
            this.cardId = cardId;
            this.incarnated = incarnated;
        }

        public override Packet Copy() => new SetIncarnatedPacket(cardId, incarnated);
    }
}

namespace KompasClient.Networking
{
    public class SetIncarnatedClientPacket : SetIncarnatedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if (incarnated) card.Incarnate();
            else card.Remove();
        }
    }
}