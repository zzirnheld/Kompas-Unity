using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetHandTargetPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int subeffIndex;

        public GetHandTargetPacket() : base(GetHandTarget) { }

        public GetHandTargetPacket(int sourceCardId, int effIndex, int subeffIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.subeffIndex = subeffIndex;
        }

        public GetHandTargetPacket(CardRestriction restriction)
            : this(restriction.Subeffect.Source.ID, restriction.Subeffect.Effect.EffectIndex, restriction.Subeffect.SubeffIndex)
        { }

        public override Packet Copy() => new GetHandTargetPacket(sourceCardId, effIndex, subeffIndex);
    }
}

namespace KompasClient.Networking
{
    public class GetHandTargetClientPacket : GetHandTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var subeff = clientGame.GetCardWithID(sourceCardId)?.Effects.ElementAt(effIndex).Subeffects[subeffIndex];
            if (subeff == null) return;
            var restriction = (subeff as DummyCardTargetSubeffect)?.cardRestriction;
            if (restriction != null)
            {
                clientGame.targetMode = Game.TargetMode.HandTarget;
                clientGame.CurrCardRestriction = restriction;
                clientGame.clientUICtrl.SetCurrState("Choose Hand Target", restriction.blurb);
            }
        }
    }
}