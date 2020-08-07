using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetEffectOptionPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int subeffIndex;

        public GetEffectOptionPacket() : base(GetEffectOption) { }

        public GetEffectOptionPacket(int sourceCardId, int effIndex, int subeffIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.subeffIndex = subeffIndex;
        }

        public GetEffectOptionPacket(ChooseOptionSubeffect subeffect)
            : this(subeffect.Source.ID, subeffect.Effect.EffectIndex, subeffect.SubeffIndex)
        { }

        public override Packet Copy() => new GetEffectOptionPacket(sourceCardId, effIndex, subeffIndex);
    }
}

namespace KompasClient.Networking
{
    public class GetEffectOptionClientPacket : GetEffectOptionPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var subeff = clientGame.GetCardWithID(sourceCardId)?.Effects.ElementAt(effIndex).Subeffects[subeffIndex] as DummyChooseOptionSubeffect;
            if (subeff != null) clientGame.clientUICtrl.ShowEffectOptions(subeff);
            //TODO catch out of bounds errors, in case of malicious packets?
        }
    }
}