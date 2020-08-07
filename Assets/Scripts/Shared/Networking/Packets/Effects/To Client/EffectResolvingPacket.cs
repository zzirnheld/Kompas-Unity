using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class EffectResolvingPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int controllerIndex;

        public EffectResolvingPacket() : base(EffectResolving) { }

        public EffectResolvingPacket(int sourceCardId, int effIndex, int controllerIndex, bool invert = false) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
        }

        public override Packet Copy() => new EffectResolvingPacket(sourceCardId, effIndex, controllerIndex);

        public override Packet GetInversion(bool known = true) => new EffectResolvingPacket(sourceCardId, effIndex, controllerIndex, invert: true);
    }
}

namespace KompasClient.Networking
{
    public class EffectResolvingClientPacket : EffectResolvingPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(sourceCardId);
            if (card == null) return;
            var eff = card.Effects.ElementAt(effIndex);
            eff.Controller = clientGame.Players[controllerIndex];
            clientGame.clientUICtrl.SetCurrState($"Resolving Effect of {card.CardName}", $"{eff.Blurb}");
        }
    }
}