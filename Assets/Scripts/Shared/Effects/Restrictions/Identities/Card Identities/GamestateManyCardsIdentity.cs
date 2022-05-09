using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextManyCardsIdentity : IContextInitializeable
    {
        public ICollection<GameCardBase> Cards { get; }
    }

    /// <summary>
    /// Identifies a collection of cards, based on the current gamestate
    /// </summary>
    public abstract class GamestateManyCardsIdentityBase : ContextInitializeableBase,
        INoActivationContextManyCardsIdentity
    {
        protected abstract ICollection<GameCardBase> AbstractCards { get; }

        public ICollection<GameCardBase> Cards
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractCards;
            }
        }
    }

    namespace GamestateManyCardsIdentities
    {
        public class FittingRestriction : GamestateManyCardsIdentityBase
        {
            public CardRestriction cardRestriction;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardRestriction.Initialize(restrictionContext.source, restrictionContext.subeffect?.Effect, restrictionContext.subeffect);
            }

            protected override ICollection<GameCardBase> AbstractCards
                => RestrictionContext.game.Cards.Where(c => cardRestriction.IsValidCard(c, default)).ToArray();
        }
    }
}