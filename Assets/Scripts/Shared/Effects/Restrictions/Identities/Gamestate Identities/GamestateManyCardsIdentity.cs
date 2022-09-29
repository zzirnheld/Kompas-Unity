using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManyCardsIdentities
    {
        public class FittingRestriction : NoActivationContextIdentityBase<ICollection<GameCardBase>>
        {
            public CardRestriction cardRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractItem
                => InitializationContext.game.Cards.Where(c => cardRestriction.IsValidCard(c, default)).ToArray();
        }
    }
}