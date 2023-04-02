using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Spaces
{
    public class PositionOf : ContextualParentIdentityBase<Space>
    {
        public IIdentity<GameCardBase> ofThisCard;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            ofThisCard.Initialize(initializationContext);
            base.Initialize(initializationContext);
        }

        protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => ofThisCard.From(context, secondaryContext).Position;
    }
}