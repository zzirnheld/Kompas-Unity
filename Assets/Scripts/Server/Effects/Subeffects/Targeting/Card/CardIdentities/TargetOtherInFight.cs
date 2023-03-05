using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.Cards;

namespace KompasServer.Effects.Subeffects
{
    public class TargetOtherInFight : AutoTargetCardIdentity
    {
        public IIdentity<GameCardBase> other = new TargetIndex();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new OtherInFight() { other = other };
            base.Initialize(eff, subeffIndex);
        }
    }
}