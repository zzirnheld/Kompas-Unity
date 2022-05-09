using KompasCore.Cards;
using KompasCore.GameCore;
using KompasServer.Effects.Identities;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Gets a single card from the current gamestate.
    /// </summary>
    public abstract class GamestateCardIdentity : ContextInitializeableBase, 
        IActivationContextCardIdentity, ISubeffectCardIdentity
    {
        protected abstract GameCardBase AbstractCardFrom(Game game, ActivationContext context);

        public GameCardBase CardFrom(Game game, ActivationContext context = default)
        {
            ComplainIfNotInitialized();
            return AbstractCardFrom(game, context);
        }

        public GameCardBase CardFrom(ActivationContext context)
            => CardFrom(context.game, context);

        public GameCardBase Card 
            => CardFrom(RestrictionContext.game, RestrictionContext.subeffect?.CurrentContext);
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

            protected override GameCardBase AbstractCardFrom(Game game, ActivationContext context)
                => ofTheseCards.CardsFrom(game, context).FirstOrDefault();
        }

        public class ThisCard : GamestateCardIdentity
        {
            protected override GameCardBase AbstractCardFrom(Game game, ActivationContext context)
                => RestrictionContext.source;
        }
    }
}