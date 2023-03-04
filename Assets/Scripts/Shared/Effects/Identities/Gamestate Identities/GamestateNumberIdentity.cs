using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Identities
{
    namespace GamestateNumberIdentities
    {
        public class Constant : NoActivationContextIdentityBase<int>
        {
            public static Constant One => new Constant { constant = 1 };

            public int constant;

            protected override int AbstractItem => constant;
        }

        public class CountCards : NoActivationContextIdentityBase<int>
        {
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cards = new GamestateManyCardsIdentities.All();

            public CardRestriction cardRestriction = new CardRestriction();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override int AbstractItem => cards.Item.Count(c => cardRestriction.IsValidCard(c, default));
        }
    }
}