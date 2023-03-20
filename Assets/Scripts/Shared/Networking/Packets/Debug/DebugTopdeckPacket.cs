using KompasCore.Cards.Movement;
using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class DebugTopdeckPacket : Packet
    {
        public int cardId;

        public DebugTopdeckPacket() : base(DebugTopdeck) { }

        public DebugTopdeckPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new DebugTopdeckPacket(cardId);
    }
}

namespace KompasServer.Networking
{
    public class DebugTopdeckServerPacket : DebugTopdeckPacket, IServerOrderPacket
    {
        public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            var card = serverGame.GetCardWithID(cardId);
            if (card == null)
                return Task.CompletedTask;
            else if (serverGame.UIController.DebugMode)
            {
                UnityEngine.Debug.LogWarning($"Debug topdecking card with id {cardId}");
                card.Topdeck();
            }
            else
            {
                UnityEngine.Debug.LogError($"Tried to debug topdeck card with id {cardId} while NOT in debug mode!");
                player.notifier.NotifyPutBack();
            }
            return Task.CompletedTask;
        }
    }
}