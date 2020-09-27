using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetBoardTargetPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int subeffIndex;

        public GetBoardTargetPacket() : base(GetBoardTarget) { }

        public GetBoardTargetPacket(int sourceCardId, int effIndex, int subeffIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.subeffIndex = subeffIndex;
        }

        public GetBoardTargetPacket(CardRestriction restriction)
            : this(restriction.Subeffect.Source.ID, restriction.Subeffect.Effect.EffectIndex, restriction.Subeffect.SubeffIndex)
        { }

        public override Packet Copy() => new GetBoardTargetPacket(sourceCardId, effIndex, subeffIndex);
    }
}

namespace KompasClient.Networking
{
    public class GetBoardTargetClientPacket : GetBoardTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var subeff = clientGame.GetCardWithID(sourceCardId)?.Effects.ElementAt(effIndex).Subeffects[subeffIndex];
            if (subeff == null) return;
            var restriction = (subeff as DummyBoardTargetSubeffect)?.cardRestriction;
            if (restriction != null)
            {
                clientGame.targetMode = Game.TargetMode.BoardTarget;
                clientGame.CurrCardRestriction = restriction;
                clientGame.clientUICtrl.SetCurrState("Choose Board Target", restriction.blurb);
                clientGame.ShowValidCardTargets(c => restriction.Evaluate(c));
            }
        }
    }
}