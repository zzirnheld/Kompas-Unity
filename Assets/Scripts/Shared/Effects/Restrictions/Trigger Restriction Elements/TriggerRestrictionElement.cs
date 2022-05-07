using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionElement
    {
        public bool primaryContext;

        protected TriggerRestriction Parent { get; private set; }

        private bool initialized = false;

        public virtual void Initialize(TriggerRestriction parent)
        {
            Parent = parent;

            initialized = true;
        }

        public bool IsValidContext(ActivationContext context, ActivationContext secondaryContext = default)
        {
            if (!initialized) throw new NotImplementedException($"You failed to initialize a new TriggerRestrictionElement of type {this.GetType()}");
            return IsValidContextLogic(context, secondaryContext);
        }

        protected abstract bool IsValidContextLogic(ActivationContext context, ActivationContext secondaryContext);
    }

    /// <summary>
    /// An element of 
    /// </summary>
    public class SpaceTriggerRestrictionElement : TriggerRestrictionElement
    {
        public SpaceRestriction spaceRestriction;
        public IActivationContextSpaceIdentity spaceIdentity;

        public override void Initialize(TriggerRestriction parent)
        {
            base.Initialize(parent);
        }

        protected override bool IsValidContextLogic(ActivationContext context, ActivationContext secondaryContext)
        {
            ActivationContext contextToConsider = primaryContext ? context : secondaryContext;
            Space space = spaceIdentity.SpaceFrom(contextToConsider);
            return spaceRestriction.IsValidSpace(space, context);
        }
    }

    public class CardTriggerRestrictionElement : TriggerRestrictionElement
    {
        public CardRestriction cardRestriction;
        public IActivationContextCardIdentity activationContextCardIdentity;


        protected override bool IsValidContextLogic(ActivationContext context, ActivationContext secondaryContext)
        {
            ActivationContext contextToConsider = primaryContext ? context : secondaryContext;
            GameCard card = activationContextCardIdentity.GameCardFromContext(contextToConsider);
            return cardRestriction.IsValidCard(card, contextToConsider);
        }
    }
}