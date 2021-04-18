using KompasClient.GameCore;
using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class AugmentActionPacket : Packet
    {
        public int cardId;
        public int x;
        public int y;

        public AugmentActionPacket() : base(AugmentAction) { }

        public AugmentActionPacket(int cardId, int x, int y) : this()
        {
            this.cardId = cardId;
            this.x = x;
            this.y = y;
        }

        public override Packet Copy() => new AugmentActionPacket(cardId, x, y);
    }
}

namespace KompasServer.Networking
{
    public class AugmentActionServerPacket : AugmentActionPacket, IServerOrderPacket
    {
        public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            if (player.Index == 1)
            {
                x = 6 - x;
                y = 6 - y;
            }
            var card = serverGame.GetCardWithID(cardId);
            await player.TryAugment(card, x, y);
        }
    }
}