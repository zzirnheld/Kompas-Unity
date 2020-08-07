using KompasCore.Networking;
using KompasServer.GameCore;

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
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            var card = serverGame.GetCardWithID(cardId);
            if (card == null) return;
            else if (serverGame.uiCtrl.DebugMode)
            {
                UnityEngine.Debug.LogWarning($"Debug topdecking card with id {cardId}");
                card.Topdeck();
            }
            else
            {
                UnityEngine.Debug.LogError($"Tried to debug topdeck card with id {cardId} while NOT in debug mode!");
                player.ServerNotifier.NotifyPutBack();
            }
        }
    }
}