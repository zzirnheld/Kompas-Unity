using KompasCore.Effects.Identities.GamestateCardIdentities;
using KompasCore.Effects.Identities.Leaf.Card;

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