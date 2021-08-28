using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class ListChoicesPacket : Packet
    {
        public int[] cardIds;

        public ListChoicesPacket() : base(ListChoicesChosen) { }

        public ListChoicesPacket(int[] cardIds) : this()
        {
            this.cardIds = cardIds;
        }

        public ListChoicesPacket(IEnumerable<GameCard> cards) : this(cards.Select(c => c.ID).ToArray()) { }

        public override Packet Copy() => new ListChoicesPacket(cardIds);
    }
}

namespace KompasServer.Networking
{
    public class ListChoicesServerPacket : ListChoicesPacket, IServerOrderPacket
    {
        public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            var choices = cardIds.Select(c => serverGame.GetCardWithID(c)).Where(c => c != null).Distinct();

            awaiter.CardListTargets = choices;
            return Task.CompletedTask;
        }
    }
}