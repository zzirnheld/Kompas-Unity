using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class MainCardAfter : TriggerContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext context)
            => context.MainCardInfoAfter;
    }
}