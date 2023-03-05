using KompasCore.Effects.Identities.ActivationContextCardIdentities;
using KompasCore.Effects.Identities.Cards;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAugmentedCard : AutoTargetCardIdentity
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new AugmentedCard() { ofThisCard = new ThisCard() };
            base.Initialize(eff, subeffIndex);
        }
    }
}