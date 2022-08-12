using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateCardIdentities
    {
        public class Any : NoActivationContextIdentityBase<GameCardBase>
        {
            public INoActivationContextManyCardsIdentity ofTheseCards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                ofTheseCards.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItem => ofTheseCards.Cards.FirstOrDefault();
        }

        public class ThisCard : NoActivationContextIdentityBase<GameCardBase>
        {
            protected override GameCardBase AbstractItem => InitializationContext.source;
        }

        public class AugmentedCard : NoActivationContextIdentityBase<GameCardBase>
        {
            public INoActivationContextIdentity<GameCardBase> ofThisCard;

            protected override GameCardBase AbstractItem
                => ofThisCard.Item.AugmentedCard;
        }
    }
}