using KompasCore.Networking;
using KompasServer.Effects;
using KompasServer.GameCore;
using System.Linq;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
    public class ActivateEffectActionPacket : Packet
    {
        public int cardId;
        public int effIndex;

        public ActivateEffectActionPacket() : base(ActivateEffectAction) { }

        public ActivateEffectActionPacket(int cardId, int effIndex) : this()
        {
            this.cardId = cardId;
            this.effIndex = effIndex;
        }

        public override Packet Copy() => new ActivateEffectActionPacket(cardId, effIndex);
    }
}

namespace KompasServer.Networking
{
    public class ActivateEffectActionServerPacket : ActivateEffectActionPacket, IServerOrderPacket
    {
        public async Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
        {
            var card = serverGame.GetCardWithID(cardId);
            if (card == null) return;
            var eff = card.Effects.ElementAt(effIndex);
            if (eff == null) return;

            await player.TryActivateEffect(eff as ServerEffect);
        }
    }
}