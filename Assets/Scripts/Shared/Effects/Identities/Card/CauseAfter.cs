using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class CauseAfter : TriggerContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
            => contextToConsider.CauseCardInfoAfter;
    }
}