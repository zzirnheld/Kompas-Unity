using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.ActivationContextCardIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetOtherInFight : AutoTargetCardIdentity
    {
        public IActivationContextIdentity<GameCardBase> other = new TargetIndex();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new OtherInFight() { other = other };
            base.Initialize(eff, subeffIndex);
        }
    }
}