using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetSpaceTargetPacket : Packet
    {
        public int sourceCardId;
        public int effIndex;
        public int subeffIndex;

        public GetSpaceTargetPacket() : base(GetSpaceTarget) { }

        public GetSpaceTargetPacket(int sourceCardId, int effIndex, int subeffIndex) : this()
        {
            this.sourceCardId = sourceCardId;
            this.effIndex = effIndex;
            this.subeffIndex = subeffIndex;
        }

        public GetSpaceTargetPacket(SpaceRestriction restriction)
            : this(restriction.Subeffect.Source.ID, restriction.Subeffect.Effect.EffectIndex, restriction.Subeffect.SubeffIndex)
        { }

        public override Packet Copy() => new GetSpaceTargetPacket(sourceCardId, effIndex, subeffIndex);
    }
}

namespace KompasClient.Networking
{
    public class GetSpaceTargetClientPacket : GetSpaceTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            var subeff = clientGame.GetCardWithID(sourceCardId)?.Effects.ElementAt(effIndex).Subeffects[subeffIndex];
            if (subeff == null) return;
            var restriction = (subeff as DummySpaceTargetSubeffect)?.spaceRestriction;
            if (restriction != null)
            {
                clientGame.targetMode = Game.TargetMode.SpaceTarget;
                clientGame.CurrSpaceRestriction = restriction;
                //TODO display based on that space
                clientGame.clientUICtrl.SetCurrState("Choose Space Target", restriction.blurb);
                clientGame.clientUICtrl.boardUICtrl.ShowSpaceTargets(space => restriction.Evaluate(space));
            }
        }
    }
}