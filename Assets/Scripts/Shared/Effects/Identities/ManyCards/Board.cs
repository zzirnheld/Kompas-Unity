using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities.ManyCards
{
    public class Board : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
    {
        protected override IReadOnlyCollection<GameCardBase> AbstractItem
            => InitializationContext.game.BoardController.Cards.ToArray();
    }
}