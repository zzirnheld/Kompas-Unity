using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class PassPriorityPacket : Packet
    {
        public PassPriorityPacket() : base(PassPriority) { }

        public override Packet Copy() => new PassPriorityPacket();
    }
}

namespace KompasServer.Networking
{
    public class PassPriorityServerPacket : PassPriorityPacket, IServerOrderPacket
    {
        public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            player.passedPriority = true;
            await serverGame.EffectsController.CheckForResponse(reset: false);
        }
    }
}