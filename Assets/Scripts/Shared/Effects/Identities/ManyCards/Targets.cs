using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities.ManyCards
{
    public class Targets : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
    {
        protected override IReadOnlyCollection<GameCardBase> AbstractItem => InitializationContext.effect.CardTargets;
    }
}