using KompasCore.Networking;
using KompasServer.GameCore;

namespace KompasCore.Networking
{
    public class DebugRehandPacket : Packet
    {
        public int cardId;

        public DebugRehandPacket() : base(DebugRehand) { }

        public DebugRehandPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new DebugRehandPacket(cardId);
    }
}

namespace KompasServer.Networking
{
    public class DebugRehandServerPacket : DebugRehandPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            var card = serverGame.GetCardWithID(cardId);
            if (card == null) return;
            else if (serverGame.uiCtrl.DebugMode)
            {
                UnityEngine.Debug.LogWarning($"Debug rehanding card with id {cardId}");
                card.Rehand();
            }
            else
            {
                UnityEngine.Debug.LogError($"Tried to debug rehand card with id {cardId} while NOT in debug mode!");
                player.ServerNotifier.NotifyPutBack();
            }
        }
    }
}