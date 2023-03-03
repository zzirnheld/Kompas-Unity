using KompasCore.Effects;
using KompasCore.Effects.Identities.GamestateManyCardsIdentities;
using KompasServer.Effects.Identities.SubeffectCardIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAugments : TargetAllCardsIdentity
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            cardsIdentity = new FittingRestriction() {
                cardRestriction = cardRestriction ?? new CardRestriction(),
                cards = new Augments() { card = new FromActivationContext() { cardFromContext = new Target() } } };
            base.Initialize(eff, subeffIndex);
        }
    }
}