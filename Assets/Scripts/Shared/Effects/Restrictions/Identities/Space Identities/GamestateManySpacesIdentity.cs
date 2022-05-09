using KompasServer.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public abstract class GamestateManySpacesIdentity : ContextInitializeableBase,
        IActivationContextManySpacesIdentity, ISubeffectManySpacesIdentity
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
        public class PositionsOfEach : GamestateManySpacesIdentity
        {
            public GamestateCardsIdentity cards;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cards.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractLogic => cards.CardsFrom(RestrictionContext.game)
                .Select(c => c.Position)
                .Where(space => space != null)
                .ToArray();
        }
    }
}