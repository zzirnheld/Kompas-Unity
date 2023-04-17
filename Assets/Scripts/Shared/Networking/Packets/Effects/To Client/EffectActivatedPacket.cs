using KompasCore.Networking;
using KompasClient.GameCore;
using System.Linq;
using KompasClient.Effects;
using UnityEngine;

namespace KompasCore.Networking
{
    public class EffectActivatedPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;

        public EffectActivatedPacket() : base(EffectActivated) { }

        public EffectActivatedPacket(int sourceCardId, int effIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
        }

        public override Packet Copy() => new EffectActivatedPacket(sourceCardId, effIndex);

        public override Packet GetInversion(bool known = true) => new EffectActivatedPacket(sourceCardId, effIndex);
    }
}

namespace KompasClient.Networking
{
    public class EffectActivatedClientPacket : EffectActivatedPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var card = clientGame.GetCardWithID(sourceCardId);
            Debug.Log($"Trying to activate effect of {sourceCardId}, which is {card}. its effect{effIndex} is {card?.Effects?.ElementAt(effIndex)}");
            if (card == null) return;
            var eff = card.Effects.ElementAt(effIndex) as ClientEffect;
            eff?.Activated();
        }
    }
}