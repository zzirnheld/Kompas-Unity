using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class Attacker : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => GetAttack(contextToConsider).attacker;
    }
}