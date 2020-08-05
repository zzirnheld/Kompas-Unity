using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;

namespace KompasCore.Networking
{
    public class PassPriorityPacket : Packet
    {
        public PassPriorityPacket() : base(ChooseEffectOption) { }

        public override Packet Copy() => new PassPriorityPacket();
    }
}

namespace KompasServer.Networking
{
    public class PassPriorityServerPacket : PassPriorityPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            player.passedPriority = true;
            serverGame.EffectsController.CheckForResponse(reset: false);
        }
    }
}