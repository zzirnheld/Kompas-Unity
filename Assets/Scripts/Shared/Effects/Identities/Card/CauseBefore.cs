using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class CauseBefore : TriggerContextualCardIdentityBase
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
            => contextToConsider.cardCauseBefore;
    }
}