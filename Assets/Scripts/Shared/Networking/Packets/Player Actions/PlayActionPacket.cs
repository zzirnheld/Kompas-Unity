using KompasClient.GameCore;
using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class PlayActionPacket : Packet
    {
        public int cardId;
        public int x;
        public int y;

        public PlayActionPacket() : base(PlayAction) { }

        public PlayActionPacket(int cardId, int x, int y) : this()
        {
            this.cardId = cardId;
            this.x = x;
            this.y = y;
        }

        public override Packet Copy() => new PlayActionPacket(cardId, x, y);
    }
}

namespace KompasServer.Networking
{
    public class PlayActionServerPacket : PlayActionPacket, IServerOrderPacket
    {
        public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            if(player.index == 1)
            {
                x = 6 - x;
                y = 6 - y;
            }
            var card = serverGame.GetCardWithID(cardId);
            await player.TryPlay(card, (x, y));
        }
    }
}