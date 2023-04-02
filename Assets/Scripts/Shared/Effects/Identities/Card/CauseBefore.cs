using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class CauseBefore : TriggerContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
            => contextToConsider.cardCauseBefore;
    }
}