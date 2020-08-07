using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetDiscardTargetPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int subeffIndex;

        public GetDiscardTargetPacket() : base(GetDiscardTarget) { }

        public GetDiscardTargetPacket(int sourceCardId, int effIndex, int subeffIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.subeffIndex = subeffIndex;
        }

        public GetDiscardTargetPacket(CardRestriction restriction)
            : this(restriction.Subeffect.Source.ID, restriction.Subeffect.Effect.EffectIndex, restriction.Subeffect.SubeffIndex)
        { }

        public override Packet Copy() => new GetDiscardTargetPacket(sourceCardId, effIndex, subeffIndex);
    }
}

namespace KompasClient.Networking
{
    public class GetDiscardTargetClientPacket : GetDiscardTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var subeff = clientGame.GetCardWithID(sourceCardId)?.Effects.ElementAt(effIndex).Subeffects[subeffIndex];
            if (subeff == null) return;
            var restriction = (subeff as DummyCardTargetSubeffect)?.cardRestriction;
            if (restriction != null)
            {
                clientGame.targetMode = Game.TargetMode.OnHold;
                clientGame.CurrCardRestriction = restriction;
                var discardToSearch = clientGame.friendlyDiscardCtrl.CardsThatFitRestriction(restriction);
                clientGame.clientUICtrl.StartSearch(discardToSearch);
                clientGame.clientUICtrl.SetCurrState("Choose Discard Target", restriction.blurb);
            }
        }
    }
}