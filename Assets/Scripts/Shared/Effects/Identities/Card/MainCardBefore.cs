using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class MainCardBefore : TriggerContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext context)
            => context.mainCardInfoBefore;
    }
}