using KompasClient.GameCore;
using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class IncarnateActionPacket : Packet
    {
        public IncarnateActionPacket() : base(IncarnateAction) { }

        public override Packet Copy() => new IncarnateActionPacket();
    }
}

namespace KompasServer.Networking
{
    public class IncarnateActionServerPacket : IncarnateActionPacket, IServerOrderPacket
    {
        public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            await player.TryIncarnate();
        }
    }
}