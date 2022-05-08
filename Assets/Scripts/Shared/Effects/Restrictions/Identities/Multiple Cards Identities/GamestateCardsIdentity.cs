using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Identifies a collection of cards, based on the current gamestate
    /// </summary>
    public abstract class GamestateCardsIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract ICollection<GameCard> CardsFromLogic(Game game, ActivationContext context);

        public ICollection<GameCard> CardsFrom(Game game, ActivationContext context = default)
            => initialized ? CardsFromLogic(game, context)
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class AllGameCardsIdentity : GamestateCardsIdentity
    {
        public CardRestriction cardRestriction;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            cardRestriction.Initialize(restrictionContext.source, restrictionContext.subeffect?.Effect, restrictionContext.subeffect);
        }

        protected override ICollection<GameCard> CardsFromLogic(Game game, ActivationContext context)
        {
            return game.Cards.Where(c => cardRestriction.IsValidCard(c, context)).ToArray();
        }
    }
}