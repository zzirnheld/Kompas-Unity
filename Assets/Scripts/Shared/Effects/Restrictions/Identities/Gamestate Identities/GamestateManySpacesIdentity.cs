using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextManySpacesIdentity : IContextInitializeable
    {
        public ICollection<Space> Spaces { get; }
    }

    /// <summary>
    /// Identifies a collection of spaces.
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// </summary>
    public abstract class GamestateManySpacesIdentityBase : ContextInitializeableBase,
        IActivationContextManySpacesIdentity, INoActivationContextManySpacesIdentity
    {
        protected abstract ICollection<Space> AbstractLogic { get; }

        public ICollection<Space> Spaces
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractLogic;
            }
        }

        public ICollection<Space> SpacesFrom(ActivationContext context) => Spaces;
    }

    namespace GamestateManySpacesIdentities
    {
        public class PositionsOfEach : GamestateManySpacesIdentityBase
        {
            public INoActivationContextManyCardsIdentity cards;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cards.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractLogic => cards.Cards
                .Select(c => c.Position)
                .Where(space => space != null)
                .ToArray();
        }
    }
}