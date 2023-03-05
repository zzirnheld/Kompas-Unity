using KompasCore.Cards;
using KompasCore.Exceptions;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{
    public class OtherInFight : ContextualParentIdentityBase<GameCardBase>
    {
        public IIdentity<GameCardBase> other;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            other.Initialize(initializationContext);
        }

        protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
        {
            Attack attack = GetAttack(toConsider(context, secondaryContext));
            var otherCard = other.From(context, secondaryContext);

            if (attack.attacker == otherCard) return attack.defender;
            if (attack.defender == otherCard) return attack.attacker;

            throw new NullCardException($"Neither card of attack {attack} was {otherCard}");
        }
    }
}