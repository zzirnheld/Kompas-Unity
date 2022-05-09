using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class ActivationContextManySpacesIdentity : ContextInitializeableBase, IContextInitializeable
    {
        protected abstract ICollection<Space> AbstractSpacesFrom(ActivationContext context);

        public ICollection<Space> SpacesFrom(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractSpacesFrom(context);
        }
    }

    namespace ActivationContextManySpacesIdentities
    {
        /// <summary>
        /// Spaces where they are in some defined relationship with respect to the other two defined spaces.
        /// For example, spaces that are between (relationship) the source card's space and the target space (two defined spaces).
        /// </summary>
        public class ThreeSpaceRelationship : ActivationContextManySpacesIdentity
        {
            public ActivationContextSpaceIdentity firstSpaceIdentity;
            public ActivationContextSpaceIdentity secondSpaceIdentity;

            public IThreeSpaceRelationship threeSpaceRelationship;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                firstSpaceIdentity.Initialize(restrictionContext);
                secondSpaceIdentity.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractSpacesFrom(ActivationContext context)
            {
                Space first = firstSpaceIdentity.SpaceFrom(context);
                Space second = secondSpaceIdentity.SpaceFrom(context);
                return Space.Spaces.Where(space => threeSpaceRelationship.Evaluate(first, second, space)).ToArray();
            }
        }
    }
}