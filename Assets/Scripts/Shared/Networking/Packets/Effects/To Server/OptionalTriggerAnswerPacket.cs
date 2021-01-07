using KompasCore.Cards;
using KompasCore.Networking;
using KompasServer.GameCore;
using KompasServer.Effects;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class OptionalTriggerAnswerPacket : Packet
    {
        public bool answer;

        public OptionalTriggerAnswerPacket() : base(OptionalTriggerResponse) { }

        public OptionalTriggerAnswerPacket(bool answer) : this()
        {
            this.answer = answer;
        }

        public override Packet Copy() => new OptionalTriggerAnswerPacket(answer);
    }
}

namespace KompasServer.Networking
{
    public class OptionalTriggerAnswerServerPacket : OptionalTriggerAnswerPacket, IServerOrderPacket
    {
        public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            awaiter.OptionalTriggerAnswer = answer;
            return Task.CompletedTask;
        }
    }
}