using KompasCore.Cards;
using KompasCore.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManyCardsIdentities
    {
        public class Limit : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<int> limit;
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                limit.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => CollectionsHelper.Shuffle(cards.From(context, secondaryContext))
                    .Take(limit.From(context, secondaryContext))
                    .ToArray();
        }
    }
}