using KompasClient.GameCore;
using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class SetDeckPacket : Packet
    {
        public string decklist = "";

        public SetDeckPacket() : base(SetDeck) { }

        public SetDeckPacket(string decklist) : this()
        {
            this.decklist = decklist;
        }

        public override Packet Copy() => new SetDeckPacket(decklist);
    }
}

namespace KompasServer.Networking
{
    public class SetDeckServerPacket : SetDeckPacket, IServerOrderPacket
    {
        public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            serverGame.SetDeck(player, decklist);
            return Task.CompletedTask;
        }
    }
}