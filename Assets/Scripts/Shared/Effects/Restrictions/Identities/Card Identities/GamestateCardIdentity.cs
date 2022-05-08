using KompasCore.Cards;
using KompasCore.GameCore;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Gets a single card from the current gamestate.
    /// </summary>
    public abstract class GamestateCardIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract GameCard CardFromLogic(Game game, ActivationContext context);

        public GameCard CardFrom(Game game, ActivationContext context = default) 
            => initialized ? CardFromLogic(game, context)
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class AnyGameCardIdentity : GamestateCardIdentity
    {
        public GamestateCardsIdentity gamestateCardsIdentity;

        protected override GameCard CardFromLogic(Game game, ActivationContext context)
            => gamestateCardsIdentity.CardsFrom(game, context).FirstOrDefault();
    }

    public class ThisCardIdentity : GamestateCardIdentity
    {
        protected override GameCard CardFromLogic(Game game, ActivationContext context)
            => RestrictionContext.source;
    }
}