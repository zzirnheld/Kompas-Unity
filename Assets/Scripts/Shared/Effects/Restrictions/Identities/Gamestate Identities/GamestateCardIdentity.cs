using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateCardIdentities
    {
        public class Any : NoActivationContextIdentityBase<GameCardBase>
        {
            public INoActivationContextIdentity<ICollection<GameCardBase>> ofTheseCards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                ofTheseCards.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItem => ofTheseCards.Item.FirstOrDefault();
        }

        public class ThisCard : NoActivationContextIdentityBase<GameCardBase>
        {
            protected override GameCardBase AbstractItem => InitializationContext.source;
        }

        public class AugmentedCard : NoActivationContextIdentityBase<GameCardBase>
        {
            public INoActivationContextIdentity<GameCardBase> ofThisCard;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                ofThisCard.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItem
                => ofThisCard.Item.AugmentedCard;
        }
    }
}