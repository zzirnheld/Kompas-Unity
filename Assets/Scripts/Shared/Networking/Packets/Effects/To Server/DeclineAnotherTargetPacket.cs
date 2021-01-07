using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;

namespace KompasCore.Networking
{
    public class DeclineAnotherTargetPacket : Packet
    {
        public DeclineAnotherTargetPacket() : base(DeclineAnotherTarget) { }

        public override Packet Copy() => new DeclineAnotherTargetPacket();
    }
}

namespace KompasServer.Networking
{
    public class DeclineAnotherTargetServerPacket : DeclineAnotherTargetPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter) => awaiter.DeclineTarget = true;
    }
}