using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetDeckTargetPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int subeffIndex;

        public GetDeckTargetPacket() : base(GetDeckTarget) { }

        public GetDeckTargetPacket(int sourceCardId, int effIndex, int subeffIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.subeffIndex = subeffIndex;
        }

        public GetDeckTargetPacket(CardRestriction restriction)
            : this(restriction.Subeffect.Source.ID, restriction.Subeffect.Effect.EffectIndex, restriction.Subeffect.SubeffIndex)
        { }

        public override Packet Copy() => new GetDeckTargetPacket(sourceCardId, effIndex, subeffIndex);
    }
}

namespace KompasClient.Networking
{
    public class GetDeckTargetClientPacket : GetDeckTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var subeff = clientGame.GetCardWithID(sourceCardId)?.Effects.ElementAt(effIndex).Subeffects[subeffIndex];
            if (subeff == null) return;
            var restriction = (subeff as DummyCardTargetSubeffect)?.cardRestriction;
            if (restriction != null)
            {
                clientGame.targetMode = Game.TargetMode.OnHold;
                UnityEngine.Debug.Log($"Deck target for Eff index: {effIndex} subeff index {subeffIndex}");
                clientGame.CurrCardRestriction = restriction;
                var toSearch = clientGame.friendlyDeckCtrl.CardsThatFitRestriction(restriction);
                clientGame.clientUICtrl.StartSearch(toSearch);
                clientGame.clientUICtrl.SetCurrState("Choose Deck Target", restriction.blurb);
            }
        }
    }
}