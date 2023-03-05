using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace ActivationContextManyCardsIdentities
    {
        public class CardsInPositions : ContextualParentIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            public IIdentity<IReadOnlyCollection<Space>> positions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<GameCardBase> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            {
                var spaces = positions.From(context, secondaryContext);
                return spaces.Select(InitializationContext.game.BoardController.GetCardAt).Where(s => s != null).ToArray();
            }
        }
    }
}