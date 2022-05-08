using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public abstract class GamestateSpacesIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract ICollection<Space> SpaceLogic();

        public ICollection<Space> Spaces() => initialized ? SpaceLogic()
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class PositionsOfGameSpaceIdentity : GamestateSpacesIdentity
    {
        public GamestateCardsIdentity cardsIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            cardsIdentity.Initialize(restrictionContext);
        }

        protected override ICollection<Space> SpaceLogic()
            => cardsIdentity.CardsFrom(RestrictionContext.game).Select(c => c.Position).ToArray();
    }
}