using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class CauseAfter : TriggerContextualCardIdentityBase
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
            => contextToConsider.CauseCardInfoAfter;
    }
}