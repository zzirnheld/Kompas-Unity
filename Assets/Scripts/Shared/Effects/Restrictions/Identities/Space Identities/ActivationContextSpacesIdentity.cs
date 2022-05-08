using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class ActivationContextSpacesIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract ICollection<Space> SpacesFromLogic(ActivationContext context);

        public ICollection<Space> SpacesFrom(ActivationContext context)
            => initialized ? SpacesFromLogic(context)
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class ThreeSpaceRelationshipContextSpacesIdentity : ActivationContextSpacesIdentity
    {
        public ActivationContextSpaceIdentity firstSpaceIdentity;
        public ActivationContextSpaceIdentity secondSpaceIdentity;

        public IThreeSpaceRelationship threeSpaceRelationship;

        protected override ICollection<Space> SpacesFromLogic(ActivationContext context)
        {
            Space first = firstSpaceIdentity.SpaceFrom(context);
            Space second = secondSpaceIdentity.SpaceFrom(context);
            return Space.Spaces.Where(space => threeSpaceRelationship.Evaluate(first, second, space)).ToArray();
        }
    }
}