using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities.ManyCards
{

    public class Augments : ContextualParentIdentityBase<IReadOnlyCollection<GameCardBase>>
    {
        public IIdentity<GameCardBase> card;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            card.Initialize(initializationContext);
        }

        protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => card.From(context, secondaryContext).Augments;
    }
}