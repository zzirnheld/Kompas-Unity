using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.Cards;

namespace KompasServer.Effects.Subeffects
{
    public class TargetTargetsAugmentedCard : AutoTargetCardIdentity
    {
        public IIdentity<GameCardBase> card = new TargetIndex();

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new AugmentedCard() { ofThisCard = card };
            base.Initialize(eff, subeffIndex);
        }
    }
}