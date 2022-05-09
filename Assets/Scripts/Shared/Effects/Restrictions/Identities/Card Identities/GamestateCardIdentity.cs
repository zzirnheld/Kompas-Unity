using KompasCore.Cards;
using KompasCore.GameCore;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Gets a single card from the current gamestate.
    /// </summary>
    public abstract class GamestateCardIdentity : ContextInitializeableBase, IContextInitializeable
    {
        protected abstract GameCard AbstractCardFrom(Game game, ActivationContext context);

        public GameCard CardFrom(Game game, ActivationContext context = default)
        {
            ComplainIfNotInitialized();
            return AbstractCardFrom(game, context);
        }
    }

    namespace GamestateCardIdentities
    {
        public class Any : GamestateCardIdentity
        {
            public GamestateCardsIdentity ofTheseCards;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                ofTheseCards.Initialize(restrictionContext);
            }

            protected override GameCard AbstractCardFrom(Game game, ActivationContext context)
                => ofTheseCards.CardsFrom(game, context).FirstOrDefault();
        }

        public class ThisCard : GamestateCardIdentity
        {
            protected override GameCard AbstractCardFrom(Game game, ActivationContext context)
                => RestrictionContext.source;
        }
    }
}