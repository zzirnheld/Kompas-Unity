using KompasCore.Cards;
using KompasCore.GameCore;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Gets a single card from the current gamestate.
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// </summary>
    public abstract class GamestateCardIdentityBase : ContextInitializeableBase, 
        IActivationContextIdentity<GameCardBase>, INoActivationContextIdentity<GameCardBase>
    {
        protected abstract GameCardBase AbstractCardFrom(Game game, ActivationContext context);

        public GameCardBase From(Game game, ActivationContext context = default)
        {
            ComplainIfNotInitialized();
            return AbstractCardFrom(game, context);
        }

        public GameCardBase From(ActivationContext context, ActivationContext secondaryContext)
            => From(context.game, context);

        public GameCardBase Item 
            => From(InitializationContext.game, InitializationContext.subeffect?.CurrentContext);
    }

    namespace GamestateCardIdentities
    {
        public class Any : GamestateCardIdentityBase
        {
            public INoActivationContextManyCardsIdentity ofTheseCards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                ofTheseCards.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractCardFrom(Game game, ActivationContext context)
                => ofTheseCards.Cards.FirstOrDefault();
        }

        public class ThisCard : GamestateCardIdentityBase
        {
            protected override GameCardBase AbstractCardFrom(Game game, ActivationContext context)
                => InitializationContext.source;
        }

        public class AugmentedCard : GamestateCardIdentityBase
        {
            public INoActivationContextIdentity<GameCardBase> ofThisCard;

            protected override GameCardBase AbstractCardFrom(Game game, ActivationContext context)
                => ofThisCard.Item.AugmentedCard;
        }
    }
}