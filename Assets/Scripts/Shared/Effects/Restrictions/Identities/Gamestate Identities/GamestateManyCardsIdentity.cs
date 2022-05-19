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
    /// Identifies a collection of cards, based on the current gamestate.
    /// (Eventually, can be used whether or not the caller does or doesn't care about an ActivationContext,
    /// but that interface doesn't even exist yet.)
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

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardRestriction.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCards
                => InitializationContext.game.Cards.Where(c => cardRestriction.IsValidCard(c, default)).ToArray();
        }
    }
}