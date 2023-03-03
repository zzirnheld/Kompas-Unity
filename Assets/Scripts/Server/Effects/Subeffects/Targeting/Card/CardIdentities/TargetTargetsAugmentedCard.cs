using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.ActivationContextCardIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetTargetsAugmentedCard : AutoTargetCardIdentity
    {
        public IActivationContextIdentity<GameCardBase> card = new TargetIndex();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new AugmentedCard() { ofThisCard = card };
            base.Initialize(eff, subeffIndex);
        }
    }
}