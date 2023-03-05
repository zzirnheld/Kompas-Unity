using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    namespace GamestateNumberIdentities
    {
        public class Constant : ContextlessLeafIdentityBase<int>
        {
            public static Constant One => new Constant { constant = 1 };

            public int constant;

            protected override int AbstractItem => constant;
        }

        public class CountCards : ContextualIdentityBase<int>
        {
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards = new GamestateManyCardsIdentities.All();

            public CardRestriction cardRestriction = new CardRestriction();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override int AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => cards.From(context, secondaryContext).Count(c => cardRestriction.IsValidCard(c, default));
        }
    }
}