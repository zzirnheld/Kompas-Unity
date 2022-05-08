using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionElement
    {
        public bool primaryContext = true;

        protected TriggerRestriction TriggerRestriction { get; private set; }
        protected RestrictionContext RestrictionContext { get; private set; }

        private bool initialized = false;

        public virtual void Initialize(TriggerRestriction triggerRestriction)
        {
            TriggerRestriction = triggerRestriction;
            RestrictionContext = new RestrictionContext(triggerRestriction.Game, triggerRestriction.ThisCard);

            initialized = true;
        }

        public bool IsValidContext(ActivationContext context, ActivationContext secondaryContext = default)
        {
            if (!initialized) throw new NotImplementedException($"You failed to initialize a new TriggerRestrictionElement of type {this.GetType()}");

            ActivationContext contextToConsider = primaryContext ? context : secondaryContext;
            return IsValidContextLogic(contextToConsider);
        }

        protected abstract bool IsValidContextLogic(ActivationContext context);
    }

    public class ThisCardInPlayTriggerRestrictionElement : TriggerRestrictionElement
    {
        protected override bool IsValidContextLogic(ActivationContext context)
            => TriggerRestriction.ThisCard.Location == CardLocation.Board;
    }

    public class CardsMatchTriggerRestrictionElement : TriggerRestrictionElement
    {
        public ActivationContextCardIdentity firstCardIdentity;
        public ActivationContextCardIdentity secondCardIdentity;

        public override void Initialize(TriggerRestriction triggerRestriction)
        {
            base.Initialize(triggerRestriction);

            firstCardIdentity.Initialize(RestrictionContext);
            secondCardIdentity.Initialize(RestrictionContext);
        }

        protected override bool IsValidContextLogic(ActivationContext context)
        {
            var first = firstCardIdentity.CardFrom(context);
            var second = secondCardIdentity.CardFrom(context);
            return first.Card == second.Card;
        }
    }


    /// <summary>
    /// An element of 
    /// </summary>
    public class SpaceRestrictionTriggerRestrictionElement : TriggerRestrictionElement
    {
        public SpaceRestriction spaceRestriction;
        public ActivationContextSpaceIdentity spaceIdentity;

        public override void Initialize(TriggerRestriction parent)
        {
            base.Initialize(parent);

            spaceIdentity.Initialize(RestrictionContext);
        }

        protected override bool IsValidContextLogic(ActivationContext context)
        {
            Space space = spaceIdentity.SpaceFrom(context);
            return spaceRestriction.IsValidSpace(space, context);
        }
    }

    public class CardRestrictionTriggerRestrictionElement : TriggerRestrictionElement
    {
        public CardRestriction cardRestriction;
        public ActivationContextCardIdentity activationContextCardIdentity;

        public override void Initialize(TriggerRestriction triggerRestriction)
        {
            base.Initialize(triggerRestriction);

            activationContextCardIdentity.Initialize(RestrictionContext);
        }

        protected override bool IsValidContextLogic(ActivationContext context)
        {
            var card = activationContextCardIdentity.CardFrom(context);
            return cardRestriction.IsValidCard(card, context);
        }
    }
}