using KompasCore.Networking;
using KompasServer.GameCore;

namespace KompasCore.Networking
{
    public class DebugDiscardPacket : Packet
    {
        public int cardId;

        public DebugDiscardPacket() : base(DebugDiscard) { }

        public DebugDiscardPacket(int cardId) : this()
        {
            this.cardId = cardId;
        }

        public override Packet Copy() => new DebugDiscardPacket(cardId);
    }
}

namespace KompasServer.Networking
{
    public class DebugDiscardServerPacket : DebugDiscardPacket, IServerOrderPacket
    {
        public void Execute(ServerGame serverGame, ServerPlayer player)
        {
            var card = serverGame.GetCardWithID(cardId);
            if (card == null) return;
            else if (serverGame.uiCtrl.DebugMode)
            {
                UnityEngine.Debug.LogWarning($"Debug discarding card with id {cardId}");
                card.Discard();
            }
            else
            {
                UnityEngine.Debug.LogError($"Tried to debug discard card with id {cardId} while NOT in debug mode!");
                card.PutBack();
            }
        }
    }
}