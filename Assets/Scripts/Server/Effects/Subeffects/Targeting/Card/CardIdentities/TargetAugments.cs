using KompasCore.Effects;
using KompasCore.Effects.Identities.Cards;
using KompasCore.Effects.Identities.ManyCards;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAugments : TargetAll
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            toSearch = new FittingRestriction() {
                cardRestriction = cardRestriction ?? new CardRestriction(),
                cards = new Augments() { card = new TargetIndex() } };
            base.Initialize(eff, subeffIndex);
        }
    }
}