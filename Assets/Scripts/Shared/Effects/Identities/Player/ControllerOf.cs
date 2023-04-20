using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Players
{
    public class ControllerOf : ContextualParentIdentityBase<Player>
    {
        public IIdentity<GameCardBase> card;
        public IIdentity<IStackable> stackable;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card?.Initialize(initializationContext);
            stackable?.Initialize(initializationContext);

            if (AllNull(card, stackable)) throw new System.ArgumentException($"Must provide something to check controller of");
        }

        protected override Player AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
        {
            if (card != null) return card.From(context, secondaryContext).Controller;
            if (stackable != null) return stackable.From(context, secondaryContext).Controller;
            throw new System.ArgumentException("huh?");
        }
    }
}