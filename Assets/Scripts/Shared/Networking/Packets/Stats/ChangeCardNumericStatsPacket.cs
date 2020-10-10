using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class ChangeCardNumericStatsPacket : Packet
    {
        public int cardId;
        public int n;
        public int e;
        public int s;
        public int w;
        public int c;
        public int a;
        public int spacesMoved;

        public ChangeCardNumericStatsPacket() : base(UpdateCardNumericStats) { }

        public ChangeCardNumericStatsPacket(int cardId, (int n, int e, int s, int w, int c, int a) stats, int spacesMoved) : this()
        {
            this.cardId = cardId;
            (n, e, s, w, c, a) =  stats;
            this.spacesMoved = spacesMoved;
        }

        public override Packet Copy() => new ChangeCardNumericStatsPacket(cardId, (n, e, s, w, c, a), spacesMoved);
    }
}

namespace KompasClient.Networking
{
    public class ChangeCardNumericStatsClientPacket : ChangeCardNumericStatsPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(cardId);
            if (card != null)
            {
                card.SetStats((n, e, s, w, c, a));
                card.SpacesMoved = spacesMoved;
            }
        }
    }
}
