using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;

namespace KompasCore.Networking
{
    public class SetEffectsXPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int x;

        public SetEffectsXPacket() : base(SetEffectsX) { }

        public SetEffectsXPacket(int sourceCardId, int effIndex, int x) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.x = x;
        }

        public override Packet Copy() => new SetEffectsXPacket(sourceCardId, effIndex, x);

        public override Packet GetInversion(bool known = true) => new SetEffectsXPacket(sourceCardId, effIndex, x);
    }
}

namespace KompasClient.Networking
{
    public class SetEffectsXClientPacket : SetEffectsXPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(sourceCardId);
            if (card != null) card.Effects.ElementAt(effIndex).X = x;
        }
    }
}