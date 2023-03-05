using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities
{
    namespace ActivationContextManyCardsIdentities
    {

        public class Augments : ContextualIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<GameCardBase> card;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                card.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => card.From(context, secondaryContext).Augments;
        }
    }
}