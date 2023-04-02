using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManyCards
{
    public class Distinct : ContextualParentIdentityBase<IReadOnlyCollection<GameCardBase>>
    {
        public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            cards.Initialize(initializationContext);
        }

        protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => cards.From(context, secondaryContext)
                .GroupBy(c => c.CardName)
                .Select(group => group.First())
                .ToArray();
    }
}