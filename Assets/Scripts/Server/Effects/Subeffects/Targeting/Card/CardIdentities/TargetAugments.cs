using KompasCore.Effects;
using KompasCore.Effects.Identities.Cards;
using KompasCore.Effects.Identities.ManyCards;
using KompasCore.Effects.Restrictions.CardRestrictionElements;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAugments : TargetAll
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            toSearch = new FittingRestriction() {
                cardRestriction = cardRestriction ?? new AlwaysValid(),
                cards = new KompasCore.Effects.Identities.ManyCards.Augments() { card = new TargetIndex() } };
            base.Initialize(eff, subeffIndex);
        }
    }
}