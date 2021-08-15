using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasCore.Networking
{
    public class SendHandSizeChoicesPacket : Packet
    {
        public int[] cardIds;

        public SendHandSizeChoicesPacket() : base(HandSizeChoices) { }

        public SendHandSizeChoicesPacket(int[] cardIds) : this()
        {
            Debug.Log($"Hand size choices {string.Join(", ", cardIds)}");
            this.cardIds = cardIds;
        }

        public override Packet Copy() => new SendHandSizeChoicesPacket(cardIds);
    }
}

namespace KompasServer.Networking
{
    public class SendHandSizeChoicesServerPacket : SendHandSizeChoicesPacket, IServerOrderPacket
    {
        public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            awaiter.HandSizeChoices = cardIds;
            return Task.CompletedTask;
        }
    }
}