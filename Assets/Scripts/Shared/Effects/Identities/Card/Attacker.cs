using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class Attacker : TriggerContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(TriggeringEventContext contextToConsider)
            => GetAttack(contextToConsider).attacker;
    }
}