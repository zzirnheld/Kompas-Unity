using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class CardBefore : TriggerContextualCardIdentityBase
    {
        public bool secondaryCard = false;

        protected override GameCardBase AbstractItemFrom(TriggeringEventContext context)
            => secondaryCard
                ? context.secondaryCardInfoBefore
                : context.mainCardInfoBefore;
    }
}