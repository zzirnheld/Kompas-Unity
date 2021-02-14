using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class EffectOptionResponsePacket : Packet
    {
        public int option;

        public EffectOptionResponsePacket() : base(ChooseEffectOption) { }

        public EffectOptionResponsePacket(int option) : this()
        {
            this.option = option;
        }

        public override Packet Copy() => new EffectOptionResponsePacket(option);
    }
}

namespace KompasServer.Networking
{
    public class EffectOptionResponseServerPacket : EffectOptionResponsePacket, IServerOrderPacket
    {
        public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            awaiter.EffOption = option;
            return Task.CompletedTask;
        }
    }
}