using KompasCore.Cards;
using KompasCore.Exceptions;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{

    public class MainCardBefore : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext context)
            => context.mainCardInfoBefore;
    }

    public class MainCardAfter : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext context)
            => context.MainCardInfoAfter;
    }

    public class CardAtPosition : ContextualIdentityBase<GameCardBase>
    {
        public IIdentity<Space> position;

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

    public class TargetIndex : ContextualLeafIdentityBase<GameCardBase>
    {
        public int index = -1;

        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => EffectHelpers.GetItem(contextToConsider.CardTargets, index);
    }

    public class CauseBefore : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.cardCauseBefore;
    }

    public class CauseAfter : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.CauseCardInfoAfter;
    }

    public class AugmentedCard : ContextualIdentityBase<GameCardBase>
    {
        public IIdentity<GameCardBase> ofThisCard;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            ofThisCard.Initialize(initializationContext);
        }

        protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            => ofThisCard.From(context, secondaryContext).AugmentedCard;
    }

    public class Attacker : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => GetAttack(contextToConsider).attacker;
    }

    public class Defender : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => GetAttack(contextToConsider).defender;
    }

    public class OtherInFight : ContextualIdentityBase<GameCardBase>
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