using KompasClient.GameCore;
using KompasCore.Networking;
using KompasServer.GameCore;

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
        public void Execute(ServerGame serverGame, ServerPlayer player) => serverGame.SetDeck(player, decklist);
    }
}