using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Identifies a collection of spaces.
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// </summary>
    public abstract class GamestateManySpacesIdentityBase : ContextInitializeableBase,
        IActivationContextIdentity<ICollection<Space>>, INoActivationContextIdentity<ICollection<Space>>
    {
        protected abstract ICollection<Space> AbstractLogic { get; }

        public ICollection<Space> Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractLogic;
            }
        }

        public ICollection<Space> From(ActivationContext context, ActivationContext secondaryContext) => Item;
    }

    namespace GamestateManySpacesIdentities
    {
        public class PositionsOfEach : GamestateManySpacesIdentityBase
        {
            public INoActivationContextManyCardsIdentity cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override ICollection<Space> AbstractLogic => cards.Cards
                .Select(c => c.Position)
                .Where(space => space != null)
                .ToArray();
        }
    }
}