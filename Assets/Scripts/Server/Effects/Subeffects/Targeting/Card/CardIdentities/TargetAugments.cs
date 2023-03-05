using KompasCore.Effects;
using KompasCore.Effects.Identities.ActivationContextCardIdentities;
using KompasCore.Effects.Identities.ActivationContextManyCardsIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAugments : TargetAllCardsIdentity
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            cardsIdentity = new FittingRestriction() {
                cardRestriction = cardRestriction ?? new CardRestriction(),
                cards = new Augments() { card = new TargetIndex() } };
            base.Initialize(eff, subeffIndex);
        }
    }
}