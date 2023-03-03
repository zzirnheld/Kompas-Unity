using KompasCore.Cards;
using KompasCore.Exceptions;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{

    public class MainCardBefore : ActivationContextIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext context)
            => context.mainCardInfoBefore;
    }

    public class MainCardAfter : ActivationContextIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext context)
            => context.MainCardInfoAfter;
    }

    public class CardAtPosition : ActivationContextIdentityBase<GameCardBase>
    {
        public IActivationContextIdentity<Space> position;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            position.Initialize(initializationContext);
        }

        protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
        {
            var finalSpace = position.From(context, secondaryContext);
            return context.game.BoardController.GetCardAt(finalSpace);
        }
    }

    public class TargetIndex : ActivationContextIdentityBase<GameCardBase>
    {
        public int index = -1;

        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => EffectHelpers.GetItem(contextToConsider.CardTargets, index);
    }

    public class CauseBefore : ActivationContextIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.cardCauseBefore;
    }

    public class CauseAfter : ActivationContextIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.CauseCardInfoAfter;
    }

    public class AugmentedCard : ActivationContextIdentityBase<GameCardBase>
    {
        public IActivationContextIdentity<GameCardBase> ofThisCard;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            ofThisCard.Initialize(initializationContext);
        }

        protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => ofThisCard.From(context, secondaryContext).AugmentedCard;
    }

    public abstract class InFight : ActivationContextIdentityBase<GameCardBase>
    {
        protected Attack GetAttack(ActivationContext context)
        {
            if (context.stackableEvent is Attack eventAttack) return eventAttack;
            if (context.stackableCause is Attack causeAttack) return causeAttack;
            else throw new NullCardException("Stackable event wasn't an attack!");
        }
    }

    public class Attacker : InFight
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => GetAttack(contextToConsider).attacker;
    }

    public class Defender : InFight
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => GetAttack(contextToConsider).defender;
    }

    public class OtherInFight : InFight
    {
        public IActivationContextIdentity<GameCardBase> other;

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