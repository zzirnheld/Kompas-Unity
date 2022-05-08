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

            ActivationContext contextToConsider = primaryContext ? context : secondaryContext;
            return IsValidContextLogic(contextToConsider);
        }

        protected abstract bool IsValidContextLogic(ActivationContext context);
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

        protected override bool IsValidContextLogic(ActivationContext context)
        {
            Space space = spaceIdentity.SpaceFrom(context);
            return spaceRestriction.IsValidSpace(space, context);
        }
    }

    public class CardTriggerRestrictionElement : TriggerRestrictionElement
    {
        public CardRestriction cardRestriction;
        public ActivationContextCardIdentity activationContextCardIdentity;

        protected override bool IsValidContextLogic(ActivationContext context)
        {
            GameCard card = activationContextCardIdentity.GameCardFromContext(context);
            return cardRestriction.IsValidCard(card, context);
        }
    }
}