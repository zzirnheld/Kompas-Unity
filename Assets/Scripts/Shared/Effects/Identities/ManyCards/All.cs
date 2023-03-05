using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManyCardsIdentities
    {
        public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
        {
            protected override IReadOnlyCollection<GameCardBase> AbstractItem => InitializationContext.game.Cards;
        }
    }
}