using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Players
{
    public class ControllerOf : ContextualParentIdentityBase<Player>
    {
        public IIdentity<GameCardBase> card;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
        }

        protected override Player AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => card.From(context, secondaryContext).Controller;
    }
}