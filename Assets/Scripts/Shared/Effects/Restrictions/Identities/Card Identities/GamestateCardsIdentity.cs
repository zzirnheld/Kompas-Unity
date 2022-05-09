using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Identifies a collection of cards, based on the current gamestate
    /// </summary>
    public abstract class GamestateCardsIdentity : ContextInitializeableBase, IContextInitializeable
    {
        protected abstract ICollection<GameCard> AbstractCardsFrom(Game game, ActivationContext context);

        public ICollection<GameCard> CardsFrom(Game game, ActivationContext context = default)
        {
            ComplainIfNotInitialized();
            return AbstractCardsFrom(game, context);
        }
    }

    namespace GamestateManyCardsIdentities
    {
        public class FittingRestriction : GamestateCardsIdentity
        {
            public CardRestriction cardRestriction;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardRestriction.Initialize(restrictionContext.source, restrictionContext.subeffect?.Effect, restrictionContext.subeffect);
            }

            protected override ICollection<GameCard> AbstractCardsFrom(Game game, ActivationContext context)
                => game.Cards.Where(c => cardRestriction.IsValidCard(c, context)).ToArray();
        }
    }
}