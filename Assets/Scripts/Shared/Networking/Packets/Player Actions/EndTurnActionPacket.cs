using KompasCore.Networking;
using KompasServer.GameCore;

namespace KompasCore.Networking
{
    public class EndTurnActionPacket : Packet
    {
        public EndTurnActionPacket() : base(EndTurnAction) { }

        public override Packet Copy() => new EndTurnActionPacket();
    }
}

namespace KompasServer.Networking
{
    public class EndTurnActionServerPacket : EndTurnActionPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            player.TryEndTurn();
        }
    }
}