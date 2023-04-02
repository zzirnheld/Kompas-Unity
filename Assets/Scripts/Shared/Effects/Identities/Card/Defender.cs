using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class Defender : TriggerContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
            => GetAttack(contextToConsider).defender;
    }
}